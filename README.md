<html>
<body>
<!--StartFragment--><html><head></head><body><h1>Structural Design Patterns in C#</h1>
<p>Structural patterns answer one question: <strong>how do I connect classes and objects so that a growing system stays flexible?</strong></p>
<p>They don't change <em>what</em> your objects do. They change <em>how objects are put together</em>: wrapped, combined, shared or hidden behind something simpler.</p>
<p>This repo walks through the seven classic Gang of Four structural patterns in C#. Each one is explained below in the same way: the problem, the solution, the benefits, real use cases and the trade-offs.</p>
<h2>Contents</h2>

Pattern | One-line idea
-- | --
Adapter | Make an incompatible class fit the interface you expect
Bridge | Separate what something is from how it is done, so each can change on its own
Composite | Treat one object and a group of objects the same way
Decorator | Add behaviour by wrapping an object
Facade | One simple front door to a complicated subsystem
Flyweight | Share common data between huge numbers of objects
Proxy | A stand-in that controls access to another object


<h2>When <em>not</em> to use a pattern</h2>
<p>Patterns solve specific problems. If the code is simple today, keep it simple. Reach for a pattern when you feel the pain it is designed to remove.</p></body></html><!--EndFragment-->
</body>
</html># Structural Design Patterns in C#

Structural patterns answer one question: **how do I connect classes and objects so that a growing system stays flexible?**

They don't change *what* your objects do. They change *how objects are put together*: wrapped, combined, shared or hidden behind something simpler.

This repo walks through the seven classic Gang of Four structural patterns in C#. Each one is explained below in the same way: the problem, the solution, the benefits, real use cases and the trade-offs.

## Contents

| Pattern | One-line idea |
| --- | --- |
| [[Adapter](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#adapter)](#adapter) | Make an incompatible class fit the interface you expect |
| [[Bridge](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#bridge)](#bridge) | Separate *what* something is from *how* it is done, so each can change on its own |
| [[Composite](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#composite)](#composite) | Treat one object and a group of objects the same way |
| [[Decorator](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#decorator)](#decorator) | Add behaviour by wrapping an object |
| [[Facade](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#facade)](#facade) | One simple front door to a complicated subsystem |
| [[Flyweight](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#flyweight)](#flyweight) | Share common data between huge numbers of objects |
| [[Proxy](https://claude.ai/chat/4c91e81e-19f9-4b8c-bafe-79ac0ddb9f74#proxy)](#proxy) | A stand-in that controls access to another object |

## What's in this repo

| Folder | Status |
| --- | --- |
| `1- Adapter` | PDF guide: *Adapter Design Pattern Architectural Guide* |
| `Bridge` | .NET console project (work in progress) |
| other patterns | Planned |

Requirements: the [[.NET 10 SDK](https://dotnet.microsoft.com/download)](https://dotnet.microsoft.com/download) (the projects target `net10.0`).

```bash
git clone https://github.com/MohamedMagdy1998/Structural.git
cd Structural
dotnet run --project Bridge
```

---

## Adapter

**Idea:** convert the interface of a class into the one your code expects.

### Problem
Your code needs a payment gateway with `ChargeAsync(Money, token)`. The legacy SDK you must use has `Charge(long cents, string currency, string token, out string reference)` and returns a magic integer. Every caller ends up knowing these quirks, and tests can't run without the real SDK.

### Solution
Define the interface *you* want (`IPaymentGateway`) and write a small adapter class that implements it by calling the legacy client. All translation (units, error codes, sync to async) lives in that one class.

```csharp
public sealed class LegacyBillingAdapter(LegacyBillingClient client) : IPaymentGateway
{
    public Task<PaymentResult> ChargeAsync(Money amount, string cardToken) { /* translate + delegate */ }
}
```

### Benefits
- Reuse code you cannot (or should not) modify.
- Vendor quirks are isolated in one place.
- Your code depends on your own interface, so swapping vendors means writing one new adapter.
- Easy to fake in unit tests.

### Real use cases
- Wrapping a legacy or third-party SDK (payments, SMS, storage).
- Making a new logging library work with `Microsoft.Extensions.Logging`.
- In .NET: `Task.Factory.FromAsync` adapts old Begin/End APIs to `Task`; `StreamReader` adapts a `Stream` to a `TextReader`.

### Trade-offs
- One extra class and one extra call per adapted type.
- Translation can be lossy: your interface may promise more than the old class can do.
- Can hide vendor features you actually need.

---

## Bridge

**Idea:** split a class into an *abstraction* and an *implementation* that vary independently.

### Problem
Reports come in kinds (Sales, Inventory) and in formats (HTML, CSV). Subclassing every combination gives `SalesHtmlReport`, `SalesCsvReport`, `InventoryHtmlReport`... Add a PDF format and you need two more classes; add a Tax report and you need three more.

### Solution
Make the report kinds one hierarchy and the formats another. A `Report` holds an `IReportRenderer` and delegates the formatting to it. The link between the two is the "bridge".

```csharp
public abstract class Report(IReportRenderer renderer)
{
    public (byte[] Data, string ContentType) Export() =>
        (renderer.Render(Title, BuildRows()), renderer.ContentType);
}
```

### Benefits
- N + M classes instead of N x M.
- Each side grows without touching the other.
- Implementation can be swapped at runtime.

### Real use cases
- Reports or documents in several output formats.
- UI controls that render on several platforms.
- In .NET: `HttpClient` (abstraction) delegates to `HttpMessageHandler` (implementation); `ILogger` delegates to `ILoggerProvider`s.

### Trade-offs
- Needs up-front design: you must spot the two dimensions early.
- More indirection; harder for newcomers to follow.
- Overkill if only one dimension really varies.

---

## Composite

**Idea:** build tree structures where single items and groups share one interface.

### Problem
A shop sells products and bundles, and bundles can contain other bundles. Pricing code becomes a type switch (`if Product ... else if Bundle ...`) that must be edited for every new kind of item, with hand-written recursion.

### Solution
Give `Product` (leaf) and `Bundle` (container) the same interface, `ICatalogItem`. A bundle answers `Price` by asking each child, so the caller never cares how deep the tree is.

```csharp
public decimal Price => _items.Sum(i => i.Price) * (1 - discountRate);
```

### Benefits
- Client code has no type checks.
- New node types plug in without changing existing code.
- Recursion comes for free.

### Real use cases
- Product bundles and nested pricing.
- Menus, file systems, org charts, UI control trees.
- In .NET: WinForms `Control.Controls`, WPF visual tree, `System.Linq.Expressions` trees.

### Trade-offs
- The shared interface can get too general.
- Hard to restrict what a container may hold.
- Cycles cause infinite recursion; deep trees cost time.

---

## Decorator

**Idea:** add behaviour to an object by wrapping it in another object with the same interface.

### Problem
A repository needs caching, logging, retries and auditing, in any combination. Subclassing gives `CachedLoggedSqlRepository` and its endless cousins; boolean flags give one class nobody can follow.

### Solution
Write each concern as a small class that implements the same interface and holds another instance of it. Stack them: logging around caching around the real repository.

```csharp
IProductRepository repo =
    new LoggingRepository(new CachingRepository(new SqlRepository(db), cache), logger);
```

### Benefits
- Add or remove behaviour without touching the real class.
- Small, single-purpose classes that are easy to test.
- Combine behaviours at runtime, in any order.

### Real use cases
- Caching, logging, retry, authorization around a service or repository.
- In .NET: `BufferedStream`, `GZipStream`, `CryptoStream`; `HttpClient`'s `DelegatingHandler` pipeline.

### Trade-offs
- Many small objects and deeper stack traces.
- Order matters: `Logging(Caching(Sql))` and `Caching(Logging(Sql))` behave differently.
- Large interfaces make every decorator long to write.

---

## Facade

**Idea:** provide one simple entry point to a complicated subsystem.

### Problem
Placing an order means reserving stock, charging a card, scheduling shipping and sending a notification, in the right order, with undo on failure. The API, the admin tool and a batch job each re-implement that, slightly differently.

### Solution
Create an `OrderFacade` with one method, `PlaceOrderAsync`. It owns the sequence and the compensation logic. Clients call one method; the subsystems stay available for advanced use.

```csharp
var confirmation = await orders.PlaceOrderAsync(request);
```

### Benefits
- Client code becomes short and readable.
- Orchestration and error handling live in one place.
- Less coupling between clients and subsystems.

### Real use cases
- Order placement, user registration, checkout flows.
- Wrapping a legacy system behind a clean API.
- In .NET: `File.ReadAllText`, `WebApplication.CreateBuilder`, `HttpClient`.

### Trade-offs
- Can grow into a "god class" that knows everything.
- Hides options that power users may need.
- Does not make a distributed workflow transactional; failure handling is still your job.

---

## Flyweight

**Idea:** share the common, unchanging part of many similar objects to save memory.

### Problem
A map draws one million trees. Each `Tree` carries its own 40 KB sprite, even though only a few tree types exist. That is roughly 40 GB of identical data.

### Solution
Split state into **intrinsic** (shared, immutable: species, colour, texture) and **extrinsic** (unique: position, scale). Keep the intrinsic part in a few shared `TreeType` objects handed out by a factory; each tree stores only its own numbers plus a reference.

```csharp
public readonly record struct Tree(float X, float Y, float Scale, TreeType Type);
```

### Benefits
- Huge memory savings when many objects look alike.
- Fewer allocations, less GC pressure.
- Shared immutable objects are thread-safe.

### Real use cases
- Games and maps (trees, particles, tiles).
- Text editors (glyph and font data).
- In .NET: `string.Intern`, `Array.Empty<T>()`, shared `Brushes` and `Pens`.

### Trade-offs
- Splitting state makes code harder to understand.
- Shared objects must stay immutable.
- Saves nothing when objects are few or mostly unique. Measure first.

---

## Proxy

**Idea:** put a stand-in in front of an object to control access to it.

### Problem
A document store is expensive to connect and holds sensitive files. Every caller creates it and must remember the permission check. One forgotten check among 60 call sites is a data leak.

### Solution
Create a proxy that implements the same interface as the real store. It checks permissions, creates the real store only on first use, then forwards the call. Callers never hold the real store, so they cannot skip the check.

```csharp
public Task<Document> LoadAsync(string id, CancellationToken ct = default)
{
    Demand("documents.read");
    return _real.Value.LoadAsync(id, ct);
}
```

### Benefits
- Access rules live in exactly one place.
- Pay for expensive objects only when they are used.
- Callers don't change.

### Real use cases
- Protection proxy: permission checks.
- Virtual proxy: lazy loading.
- Remote proxy: gRPC and WCF client stubs.
- In .NET: EF Core lazy-loading proxies, `System.Reflection.DispatchProxy`.

### Trade-offs
- Extra indirection and a new class per interface.
- A call that looks local may be slow or remote.
- Lazy creation moves failures to unexpected moments.

---

## Adapter vs Decorator vs Proxy

These three look almost the same: a wrapper that holds the object it wraps. The difference is **intent**.

| Pattern | Interface | Purpose |
| --- | --- | --- |
| Adapter | Changes it | Translate |
| Decorator | Keeps it | Add behaviour |
| Proxy | Keeps it | Control access |

## Quick comparison

| Pattern | Use it when... | Watch out for |
| --- | --- | --- |
| Adapter | Interfaces don't match | Leaky translation |
| Bridge | Two things vary independently | Up-front design cost |
| Composite | Data is a tree | An interface that is too general |
| Decorator | You need stackable behaviours | Order-dependent layers |
| Facade | A subsystem is too complicated | Becoming a god class |
| Flyweight | Millions of similar objects | Shared state being mutated |
| Proxy | Access must be controlled or deferred | Hidden latency |

## When *not* to use a pattern

Patterns solve specific problems. If the code is simple today, keep it simple. Reach for a pattern when you feel the pain it is designed to remove.
