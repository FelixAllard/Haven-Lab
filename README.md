# Branch Naming Convention

## Purpose
To ensure consistency and clarity in our git repository, we follow a standard for naming branches. This allows us to easily identify the purpose and progress of different branches within the project.

## Branch Naming Format

Each branch name should follow this structure:

```<type>/(<username>)-<ticket-number>-<short-description>```

Where:
- `<type>`: Describes the type of work being done. For example:
    - `feat`: A new feature.
    - `bug`: A bug fix.
    - `chore`: Routine tasks or updates.
    - `docs`: Documentation changes.
    - `test`: Unit or integration tests.
    - `refactor`: Code refactoring.
    - `style`: Style-related changes (e.g., formatting).

- `(<username>)`: Your username or initials to identify the contributor (enclosed in parentheses).

- `<ticket-number>`: The identifier of the task or issue being worked on (e.g., a JIRA ticket number).

- `<short-description>`: A brief, concise description of the task or feature being worked on.

## Example Branch Names

Here are a few examples to illustrate the format:

- `feat/(Felix)-51-Customer-can-access-front-end`: A new feature where Felix is implementing the ability for a customer to access the front end, associated with ticket 51.
- `fix/(Alice)-78-Fix-login-issue`: Alice is fixing a login issue as described in ticket 78.
- `chore/(John)-23-Update-dependencies`: John is updating the dependencies as part of ticket 23.
- `docs/(Felix)-45-Update-API-docs`: Felix is updating the API documentation as described in ticket 45.

## Benefits of This Convention

- **Clarity**: It’s easy to identify the purpose of a branch at a glance.
- **Traceability**: Associating branches with specific tickets or tasks makes it easy to trace changes back to specific issues or features.
- **Consistency**: Having a consistent naming format prevents confusion and helps with collaboration.

## Conclusion

Following this branch naming convention ensures that our workflow remains organized and manageable. Please adhere to this structure when creating new branches.
