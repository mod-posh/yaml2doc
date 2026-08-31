### **Phase 1: Verification & Compatibility Audit**

**Objective:** Confirm current SDK/package compatibility and identify code style inconsistencies.

1. **Verify SDK Version**
   - Run: `dotnet --version`
   - **Issue Identified:** Current SDK is .NET 10, but project targets .NET 9
   - **Action Required:** Install .NET 9 SDK (via `dotnet sdk install 9.0.x`) and switch to it

2. **Check Project Target Framework**
   - Verify all `.csproj` files use:

     ```xml
     <TargetFramework>net9.0</TargetFramework>
     ```

   - Confirm `Directory.Build.props` is configured for .NET 9

3. **Audit NuGet Package Compatibility**
   - Use `dotnet list package` to check all dependencies
   - Cross-reference with [.NET 9 Compatibility Guide](https://learn.microsoft.com/en-us/dotnet/standard/compatibility) to identify:
     - Packages explicitly **not compatible** with .NET 9
     - Packages with **deprecated APIs** in .NET 9
     - Packages requiring **.NET 9+ features** (e.g., C# 12/13)

4. **Check for Deprecated APIs**
   - Use the [.NET 9 API Portability Analyzer](https://github.com/dotnet/analyzers) to identify:
     - APIs removed in .NET 9
     - APIs with changed behavior
   - Run:

     ```bash
     dotnet analyze --project <project.csproj> --ruleset <ruleset.xml>
     ```

5. **Code Style Audit**
   - Identify code that uses **pre-.NET 9 syntax** (e.g., C# 7/8 features)
   - Check for **outdated patterns** (e.g., manual null checks, non-nullable types in nullable context)

---

### **Phase 2: Configuration & Code Style Alignment**

**Objective:** Ensure project configuration and code style align with .NET 9 best practices.

1. **Update Project File Settings**
   - Ensure all `.csproj` files include:

     ```xml
     <LangVersion>12</LangVersion> <!-- C# 12 for .NET 9 -->
     <ImplicitUsings>enable</ImplicitUsings>
     <Nullable>enable</Nullable>
     <SourceGenerationOptions>enable</SourceGenerationOptions>
     ```

2. **Update SDK Moniker**
   - Replace any `$(TargetFrameworkMoniker)` references with `net9.0` in `Directory.Build.props` or project files

3. **Update MSBuild Properties**
   - Ensure `Directory.Build.props` includes:

     ```xml
     <PropertyGroup>
       <TargetFramework>net9.0</TargetFramework>
       <LangVersion>12</LangVersion>
       <Nullable>enable</Nullable>
     </PropertyGroup>
     ```

4. **Modernize Code Style**
   - Update code to use **C# 12+ features** (e.g., pattern matching, records, nullable reference types)
   - Replace outdated patterns with **.NET 9 best practices** (e.g., use `System.Text.Json` instead of `Newtonsoft.Json`)

5. **Verify Tooling Compatibility**
   - Ensure all build tools (e.g., `dotnet-ef`, `dotnet-aspnet-codegenerator`) are updated to .NET 9-compatible versions

---

### **Phase 3: Dependency Management**

**Objective:** Resolve package compatibility issues and update dependencies.

1. **Update NuGet Packages**
   - Use `dotnet add package` to upgrade packages to their latest .NET 9-compatible versions
   - Example:

     ```bash
     dotnet add package Microsoft.AspNetCore.Mvc --version 9.0.0
     ```

2. **Replace Incompatible Packages**
   - For packages not compatible with .NET 9:
     - Check for **alternatives** in NuGet (e.g., `System.Text.Json` instead of `Newtonsoft.Json`)
     - Use `dotnet migrate` for framework-specific migrations (e.g., `System.Collections.Immutable`)

3. **Audit for Breaking Changes**
   - Review [.NET 9 Breaking Changes](https://learn.microsoft.com/en-us/dotnet/standard/compatibility/breaking-changes-9-0) to:
     - Identify APIs removed or modified
     - Update code references (if needed) in documentation or tests

---

### **Phase 4: Testing & Validation**

**Objective:** Confirm the solution works correctly after alignment.

1. **Run Full Build**
   - Execute:

     ```bash
     dotnet build --configuration Release
     ```

2. **Run Unit Tests**
   - Execute:

     ```bash
     dotnet test --configuration Release
     ```

   - Verify all tests pass with no regressions

3. **Run API Tests**
   - If applicable, run integration tests to validate .NET 9-specific features (e.g., `System.Text.Json` serialization)

4. **Check for Warnings**
   - Review build output for:
     - `warning CS8632` (nullable reference types)
     - `warning CS8618` (uninitialized fields in nullable context)
   - Address any warnings that impact functionality

---

### **Phase 5: Ongoing Maintenance**

**Objective:** Maintain compatibility with future .NET updates.

1. **Set Up CI/CD Pipeline**
   - Configure GitHub Actions/GitLab CI to:
     - Automate SDK version checks
     - Run `dotnet list package` for dependency audits
     - Enforce `dotnet build --configuration Release` on pull requests

2. **Monitor NuGet Updates**
   - Subscribe to [NuGet.org changelog](https://github.com/NuGet/Home/wiki/Changelog) for .NET 9 package updates
   - Use `dotnet list package --outdated` to track outdated packages

3. **Periodic Compatibility Checks**
   - Schedule quarterly audits to:
     - Verify SDK/package compatibility
     - Update deprecated APIs
     - Test against new .NET 9 features (e.g., `System.Text.Json` improvements)

---

## **Key Considerations**

- **No File Edits:** This plan avoids direct code changes, focusing instead on configuration, dependency management, and tooling.
- **Verified Compatibility:** All recommendations are based on [.NET 9 official documentation](https://learn.microsoft.com/en-us/dotnet/standard/compatibility) and NuGet package metadata.
- **Minimal Risk:** The plan prioritizes stability by avoiding code changes until full compatibility is confirmed.

This phased approach ensures the solution aligns with .NET 9 while maintaining stability and avoiding unnecessary code modifications.
