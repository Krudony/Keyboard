# Hotkey-Based Thai-English Text Converter - Specification

## ADDED Requirements

### Requirement: Global Hotkey Registration
The system SHALL register a global hotkey (default: Ctrl+Shift+Space) that triggers text conversion regardless of which application is active.

#### Scenario: Hotkey triggers in Notepad
- **WHEN** user presses Ctrl+Shift+Space while typing in Notepad
- **THEN** conversion is triggered immediately

#### Scenario: Hotkey triggers in web browser
- **WHEN** user presses Ctrl+Shift+Space in a web form
- **THEN** conversion works without opening a new window

#### Scenario: Hotkey works when application is unfocused
- **WHEN** user has typed text but browser is not in focus
- **THEN** user can still press Ctrl+Shift+Space and text is converted

---

### Requirement: Clipboard-Based Text Conversion
The system SHALL get text from the clipboard, convert it, and paste it back using native Windows clipboard operations.

#### Scenario: Convert text from clipboard
- **WHEN** user copies "hello" and presses hotkey
- **THEN** clipboard contains converted Thai text
- **AND** user can paste result with Ctrl+V

#### Scenario: Preserve formatting
- **WHEN** clipboard contains mixed text (Thai + English + numbers)
- **THEN** only convertible characters are changed, others preserved

#### Scenario: Handle empty clipboard
- **WHEN** user presses hotkey with nothing in clipboard
- **THEN** system shows notification "Nothing to convert"

---

### Requirement: Thai↔English Character Mapping
The system SHALL map Thai and English QWERTY keyboard characters bidirectionally with 95%+ accuracy on common words.

#### Scenario: Convert common English word to Thai
- **WHEN** user copies "hello"
- **AND** presses hotkey
- **THEN** result is Thai equivalent (based on QWERTY layout)

#### Scenario: Convert common Thai word to English
- **WHEN** user copies Thai text "สวัสดี"
- **AND** presses hotkey
- **THEN** result is English equivalent

#### Scenario: Auto-detect conversion direction
- **WHEN** user has Thai text in clipboard
- **AND** presses hotkey
- **THEN** system automatically converts Thai→English (not English→Thai)

---

### Requirement: Conversion Notification
The system SHALL display a brief notification showing the conversion result.

#### Scenario: Show success notification
- **WHEN** conversion completes successfully
- **THEN** floating tooltip shows "✓ Converted to Thai"
- **AND** tooltip disappears after 2 seconds

#### Scenario: Show error notification
- **WHEN** conversion fails (invalid characters)
- **THEN** show "✗ Cannot convert this text"

#### Scenario: Show result preview
- **WHEN** user presses hotkey
- **THEN** notification shows: "Original: hello → Thai: สวัสดี"

---

### Requirement: System Tray Control
The system SHALL provide a system tray icon with quick access to controls.

#### Scenario: Show tray icon
- **WHEN** application starts
- **THEN** system tray displays IME converter icon

#### Scenario: Toggle converter on/off
- **WHEN** user right-clicks tray icon
- **THEN** shows "Enable/Disable" option
- **AND** hotkey only works when enabled

#### Scenario: Access settings from tray
- **WHEN** user clicks tray icon
- **THEN** opens settings dialog

---

### Requirement: Configuration File Support
The system SHALL support a simple config.json for hotkey customization.

#### Scenario: Customize hotkey
- **WHEN** user edits config.json with "hotkey": "Alt+Shift+C"
- **AND** restarts application
- **THEN** Alt+Shift+C becomes active hotkey

#### Scenario: Enable/disable auto-paste
- **WHEN** user sets "autoPaste": true in config
- **THEN** converted text is automatically pasted to application
- **AND** when false, only clipboard is updated (manual paste)

---

### Requirement: Zero Installation Friction
The system SHALL run as a standalone .exe with no installation, registry, or admin requirements.

#### Scenario: Run directly from .exe
- **WHEN** user downloads KeyboardConverter.exe
- **AND** double-clicks it
- **THEN** application starts immediately without installer

#### Scenario: Portable across USB
- **WHEN** user copies .exe to USB drive
- **AND** runs on different computer
- **THEN** works without installation

#### Scenario: No elevated permissions needed
- **WHEN** user is standard (non-admin) Windows user
- **THEN** application runs without UAC prompt

---

### Requirement: Fast Conversion Performance
The system SHALL convert text in < 100ms total latency.

#### Scenario: Convert 50-character string instantly
- **WHEN** user presses hotkey with Thai text in clipboard
- **AND** text is 50 characters long
- **THEN** conversion completes in < 100ms
- **AND** result is pasted or shown in notification

#### Scenario: Handle large text (500+ characters)
- **WHEN** user has large clipboard content
- **THEN** conversion completes in < 200ms

---

### Requirement: Works in All Applications
The system SHALL work in any Windows application that supports clipboard operations.

#### Scenario: Works in Microsoft Office
- **WHEN** user types in Word, Excel, or PowerPoint
- **THEN** hotkey converts text from clipboard

#### Scenario: Works in web browsers
- **WHEN** user types in Chrome, Firefox, or Edge
- **THEN** hotkey works in form fields, chat boxes, etc.

#### Scenario: Works in code editors
- **WHEN** user types in VS Code, Visual Studio, or Sublime
- **THEN** hotkey converts code comments and strings

#### Scenario: Works in secure contexts
- **WHEN** user types in password field
- **THEN** hotkey still works (user controls when to trigger)

---

## MODIFIED Requirements

(None - this is a new approach)

## REMOVED Requirements

(None - this completely replaces the IME approach)
