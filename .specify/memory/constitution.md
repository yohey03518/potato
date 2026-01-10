<!--
SYNC IMPACT REPORT
Version: 0.0.0 -> 1.0.0
- Status: Initial Ratification
- Principles Added:
  - I. Financial Integrity & Precision (Money handling)
  - II. Test-Driven Development (TDD)
  - III. Clean Code & Self-Documentation
  - IV. SOLID Architecture
  - V. Spec-Driven Development (SDD)
  - VI. Observability & Traceability
  - VII. Dependency Minimalism
- Sections Added: Technology Standards, Governance & AI Collaboration
- Templates Status:
  - .specify/templates/plan-template.md: ✅ Compatible
  - .specify/templates/spec-template.md: ✅ Compatible
  - .specify/templates/tasks-template.md: ✅ Compatible (TDD explicitly supported)
- TODOs: None
-->

# Potato Financial System Constitution

## Core Principles

### I. Financial Integrity & Precision
**Money-related logic MUST use high-precision types (e.g., `decimal`), never floating-point types (float/double).** All financial calculations MUST be covered by comprehensive unit tests including edge cases (rounding, overflow). Audit trails for transactions are mandatory to ensure financial data integrity.

### II. Test-Driven Development (TDD)
**NON-NEGOTIABLE: Tests MUST be written before implementation.** The Red-Green-Refactor cycle is strictly enforced. All core logic MUST be covered by unit tests. Tests serve as the primary validation of the specification and ensure system reliability.

### III. Clean Code & Self-Documentation
**Code MUST be self-documenting.** Variable and method names should clearly explain intent. Comments should explain "Why" a decision was made, not "What" the code is doing. Adherence to standard naming conventions and clean code practices is mandatory.

### IV. SOLID Architecture
**System design MUST adhere to SOLID principles.** Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion principles are to be strictly followed to ensure maintainability and scalability.

### V. Spec-Driven Development (SDD)
**Implementation starts only after a clear specification is defined.** All user prompts and AI interactions involved in spec generation MUST be recorded to ensure full traceability of requirements. The project follows a strict specification-first workflow.

### VI. Observability & Traceability
**Structured logging is mandatory.** Logs MUST be structured (e.g., JSON) and provide enough context to trace system execution and logic flows completely. It must be possible to debug production issues via logs alone.

### VII. Dependency Minimalism
**External HTTP APIs MUST be integrated directly.** Use standard libraries (e.g., `HttpClient`) rather than third-party SDK wrappers, unless the SDK provides critical complexity management that cannot be reasonably replicated. This ensures control and reduces dependency bloat.

## Technology Standards

**Platform**: The project MUST use the latest **.NET LTS (Long Term Support)** version.
**Language**: C# is the primary language.
**Testing**: xUnit or NUnit is recommended, ensuring compatibility with TDD workflows.

## Governance & AI Collaboration

**AI Proactivity**: AI agents are explicitly empowered and encouraged to proactively propose amendments to this constitution if gaps, inconsistencies, or opportunities for improvement are identified during development.

**Auditability**: All user prompts driving changes must be recorded.

## Governance

**Supremacy**: This Constitution supersedes all other technical practices.
**Amendments**: Changes require documentation and approval. Versioning follows Semantic Versioning (MAJOR.MINOR.PATCH).

**Version**: 1.0.0 | **Ratified**: 2026-01-10 | **Last Amended**: 2026-01-10