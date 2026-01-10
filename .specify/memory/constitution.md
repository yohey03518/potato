<!--
SYNC IMPACT REPORT
Version: 1.0.0 -> 1.1.0
- Status: Amendment
- Principles Added:
  - VIII. Intent-Driven Git History (Conventional Commits + "Why" focus)
- Governance Added:
  - Protected Artifacts clause (AI restriction on infrastructure dirs)
- Templates Status:
  - .specify/templates/plan-template.md: ✅ Compatible
  - .specify/templates/spec-template.md: ✅ Compatible
  - .specify/templates/tasks-template.md: ✅ Compatible
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

### VIII. Intent-Driven Git History
**Commit messages MUST follow the Conventional Commits specification.** The subject line MUST describe the **"WHY"** (intent/business reason) rather than the "WHAT" (technical change). If the context for the "WHY" is missing, the AI agent MUST halt and request clarification from the user before committing.

## Technology Standards

**Platform**: The project MUST use the latest **.NET LTS (Long Term Support)** version.
**Language**: C# is the primary language.
**Testing**: xUnit or NUnit is recommended, ensuring compatibility with TDD workflows.

## Governance & AI Collaboration

**AI Proactivity**: AI agents are explicitly empowered and encouraged to proactively propose amendments to this constitution if gaps, inconsistencies, or opportunities for improvement are identified during development.

**Auditability**: All user prompts driving changes must be recorded.

**Protected Artifacts**: The AI agent is **STRICTLY PROHIBITED** from modifying files in `.gemini/`, `.specify/scripts/`, and `.specify/templates/` unless explicitly commanded by the user. These directories contain critical agent infrastructure.

## Governance

**Supremacy**: This Constitution supersedes all other technical practices.
**Amendments**: Changes require documentation and approval. Versioning follows Semantic Versioning (MAJOR.MINOR.PATCH).

**Version**: 1.1.0 | **Ratified**: 2026-01-10 | **Last Amended**: 2026-01-10
