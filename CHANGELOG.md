## 0.1.14 (20-05-2020):

- UnhandledExceptionMiddleware no longer swallows exceptions that happen during response body streaming (that could prevent Kestrel from flushing its output buffers).
- UnhandledExceptionMiddleware now clears any custom headers the app might have set when responding with HTTP 500.
- TracingMiddleware now formats trace ids in response header without dashes ("N" format).
- UsePathBase middleware is now registered before Vostok middlewares --> ping API works with base URL prefix.
- Throttling provider is now registered in DI container even if ThrottlingMiddleware is disabled in case user adds it back manually in a different position.

## 0.1.13 (01-05-2020):

- User application classes can now override `DoDispose` method to perform cleanup.
- Added a couple of functional tests for middlewares.

## 0.1.12 (30-04-2020):

Fill headers in ping api middleware.

## 0.1.11 (01-04-2020):

https://github.com/vostok/applications.aspnetcore/issues/19

## 0.1.10 (24-03-2020):

Do not explicitly disallow synchronous I/O in Kestrel, rely on Asp.NET Core defaults.

## 0.1.9 (19-03-2020):

Use Microsoft constant from `vostok.logging.microsoft` module.

## 0.1.8 (13-03-2020):

Major changes in this release:
* Multitargeting. In addition to `netcoreapp3.1` we now also target `netstandard2.0` and Asp.NET Core 2.1 (see https://github.com/vostok/applications.aspnetcore/issues/8). It's also reflected in Cement `module.yaml` as two new module configurations: `v3_1` (default) and `v2_1`. This allows to use the library in .NET Framework applications.
* Built-in middlewares no longer implement `IMiddleware` interface.
* Built-in middlewares are no longer instantiated manually.
* Built-in middlewares now use options pattern to receive configuration from DI container.
* Built-in middleware classes are all public now.
* Built-in middlewares can now be disabled, both entirely and selectively.
* Built-in middlewares can now be added with public `IApplicationBuilder` extensions.
* Significant internal refactoring necessitated by multitargeting approach.

Minor changes:
* (**Breaking change!**) `ThrottlingMetricsOptions` have been moved from `ThrottlingSettings` to an `IVostokThrottlingBuilder` property.
* `UnhandledExceptionMiddleware` now has its own settings class.
* All middlewares now enrich their log instances with source context.
* `LoggingMiddleware`, `TracingMiddleware` and `ThrottlingMiddleware` now tolerate absence of the `FillRequestInfoMiddleware`.
* `LoggingMiddleware` now also logs response completion time, which includes the time it takes to send data to the client (save for small buffered writes).
* Added xml-docs for `IVostokAspNetCoreApplicationBuilder` methods.
* Added configuration of generic host shutdown timeout based on Vostok environment.

## 0.1.7 (07-03-2020):

* https://github.com/vostok/applications.aspnetcore/issues/9
* https://github.com/vostok/applications.aspnetcore/issues/10

## 0.1.6 (03-03-2020):

* https://github.com/vostok/applications.aspnetcore/issues/6
* https://github.com/vostok/applications.aspnetcore/issues/7

## 0.1.5 (02-03-2020):

Added `VostokNetCoreApplication` instead of `DisableWebHost`.

## 0.1.4 (03-02-2020):

Added `UseCustomPropertyQuota` extension to configure throttling by custom property.

## 0.1.3 (30-01-2020):

Extract commit hash from calling assembly instead of entry assembly.

## 0.1.2 (30-01-2020):

Added `VostokAspNetCoreApplication` without `Startup`.

## 0.1.1 (28-01-2020):

* Added an option to disable WebHost.
* Added arbitrary customization of generic host.
* Added an extension to register IHostedServices.

## 0.1.0 (18-01-2020): 

Initial prerelease.
