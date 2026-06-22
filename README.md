# PubSubAssessment

A small C# solution demonstrating the Publish/Subscribe pattern applied to a
real-world scenario: gold price updates flowing through a transformation step
and being pushed out to independent subscribers.

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

---

## How to run

```bash
# From the solution root
dotnet run --project PubSubAssessment.App
```

Expected output (prices are random, yours will differ):

```
[09:14:01] Gold: 2002.31 USD (Up)
  ALERT: gold moved Up to 2002.31 USD
[09:14:01] Gold: 2001.87 USD (Stable)
[09:14:01] Gold: 1998.44 USD (Down)
  ALERT: gold moved Down to 1998.44 USD
```

## How to run tests

```bash
# From the solution root
dotnet test
```

---

## Project structure

```
PubSubAssessment/
│
├── PubSubAssessment.sln
│
├── PubSubAssessment.Core/                   # class library, no outgoing project refs
│   ├── Models/
│   │   ├── RawRecord.cs                     # shape of data before transformation
│   │   └── TransformedRecord.cs             # shape of data after transformation
│   ├── Transformation/
│   │   ├── IDataTransformer.cs              # transform contract
│   │   └── GoldPriceMovementTransformer.cs  # classifies price movement (Up/Down/Stable)
│   ├── PubSub/
│   │   ├── IPublisher.cs                    # subscribe / unsubscribe / publish contract
│   │   ├── ISubscriber.cs                   # OnNext contract
│   │   └── InMemoryBroker.cs               # in-memory pub/sub engine
│   └── Subscribers/
│       ├── ConsoleDisplaySubscriber.cs      # displays every update
│       └── MovementAlertSubscriber.cs       # alerts only on Up or Down movement
│
├── PubSubAssessment.App/                    # entry point, composition root only
│   └── Program.cs
│
└── PubSubAssessment.Tests/                  # xUnit test project, references Core only
    ├── GoldPriceMovementTransformerTests.cs
    ├── InMemoryBrokerTests.cs
    └── ConsoleDisplaySubscriberTests.cs
```

---

## How the data flows

```
Program.cs            RawRecord          GoldPriceMovement        InMemoryBroker
(generates ticks) ──────────────▶       Transformer        ──────────────▶
                                   (classifies movement)      (fan-out to all
                                                               subscribers)
                                                                    │
                                              ┌─────────────────────┤
                                              ▼                     ▼
                                  ConsoleDisplaySubscriber  MovementAlertSubscriber
                                  (displays every update)   (alerts on Up/Down only)
```

---

## Design decisions

### Core has no outgoing dependencies
`PubSubAssessment.Core` does not reference `App` or `Tests`, and has no
dependency on `Console`, file I/O, or any external library. This is the design
decision that makes every piece unit-testable in isolation — tests in
`PubSubAssessment.Tests` exercise plain objects with no environmental setup.

### TextWriter injection on subscribers
Both subscribers accept a `TextWriter` in their constructor (defaulting to
`Console.Out`) rather than calling `Console.WriteLine` directly. This keeps
display logic testable without capturing real console output — tests pass a
`StringWriter` instead.

### RawRecord carries PreviousPrice
The transformer is kept pure and stateless by making `PreviousPrice` an
explicit field on `RawRecord` rather than having the transformer track "last
seen price" internally. The trade-off is that `Program.cs` owns the only
mutable state in the system (the rolling `previousPrice` variable), which
is a deliberate choice — composition roots are an appropriate place for
wiring-level state.

### Stable threshold is ±0.1%
A movement within ±0.1% of the previous price is classified as `Stable` rather
than using zero-tolerance comparison. The threshold lives as a named constant
(`StableThresholdPercentage`) in `GoldPriceMovementTransformer` — visible,
easy to find, and easy to promote to a constructor parameter if it ever needs
to be configurable per-deployment.

### No DI container
Dependencies are composed manually in `Program.cs`. For an exercise of this
scope, a DI container would add configuration overhead without adding clarity.
The same constructor-injection pattern used here would wire directly into any
standard DI container (e.g. `Microsoft.Extensions.DependencyInjection`) with
minimal change.

---

## Known limitations

### No subscriber fault isolation
If a subscriber's `OnNext` throws, the `foreach` loop in `InMemoryBroker`
stops and subsequent subscribers do not receive that publish. In a production
context each subscriber call would be wrapped in a try/catch with appropriate
logging, so a misbehaving subscriber cannot disrupt the others.

### Not thread-safe
`InMemoryBroker` uses a plain `List<T>` internally. Concurrent calls to
`Subscribe` and `Publish` from multiple threads are not safe. A
`ConcurrentBag<T>` or a lock around list mutation would address this.

---

## How this could be extended

**Async dispatch** — `Publish` could become `Task PublishAsync` with each
subscriber called via `await`, enabling non-blocking delivery and natural
integration with `CancellationToken` for graceful shutdown.

**Subscriber fault isolation** — wrapping each `subscriber.OnNext(data)` call
in a try/catch inside the broker loop would prevent one failing subscriber from
blocking delivery to the rest, and open the door to retry or dead-letter
strategies.

**Multiple topics / event types** — the broker could maintain a
`Dictionary<Type, List<ISubscriber>>` keyed by message type, so a single
broker instance handles many event types without subscribers receiving
everything regardless of relevance.

**External message broker** — replacing `InMemoryBroker` with an implementation
backed by a real message bus (RabbitMQ, Azure Service Bus, etc.) would require
only a new `IPublisher<T>` implementation — no changes to subscribers,
transformer, or models, since they all depend on the interface, not the
in-memory concrete class.

**Thread safety** — replacing `List<ISubscriber<T>>` with a `ConcurrentBag<T>`
or wrapping mutations in a lock would make the broker safe for multi-threaded
publishers and dynamic subscriber registration.