# Simplified Hotkey Text Converter - Implementation Tasks

## Phase 1: Core MVP (Day 1-2)

- [x] 1.1 Create new console application project: `KeyboardTextConverter.csproj`
- [x] 1.2 Implement `HotkeyManager.cs` - Register Ctrl+Shift+Space globally
- [x] 1.3 Implement `ThaiEnglishConverter.cs` - Character mapping (reuse from IME version)
- [x] 1.4 Implement `ClipboardHandler.cs` - Get/set clipboard text safely
- [x] 1.5 Implement `NotificationWindow.cs` - Show floating tooltip with result
- [x] 1.6 Implement `Program.Main()` - Initialize all components and run event loop
- [x] 1.7 Create `config.json` - Default hotkey configuration

**Acceptance Criteria:**
- ✅ Pressing Ctrl+Shift+Space converts clipboard text
- ✅ Works in Notepad, Word, VS Code, Chrome
- ✅ Notification shows result
- ✅ No errors or crashes in basic testing

## Phase 2: Polish & UX (Day 3)

- [ ] 2.1 Add system tray icon with context menu
- [ ] 2.2 Implement tray menu: Enable/Disable, Settings, Exit
- [ ] 2.3 Create `SettingsWindow.cs` - Simple dialog for hotkey customization
- [ ] 2.4 Implement auto-paste option (convert + auto-paste vs manual paste)
- [ ] 2.5 Add notification sound/visual feedback option
- [ ] 2.6 Test on actual user machine with real Thai typing

**Acceptance Criteria:**
- ✅ Tray icon works correctly
- ✅ Settings dialog saves and loads hotkey
- ✅ User can enable/disable via tray
- ✅ Tested with at least 10 Thai-English word pairs

## Phase 3: Testing & Validation (Day 4)

- [ ] 3.1 Write unit tests for ThaiEnglishConverter (50+ test cases)
- [ ] 3.2 Write integration tests: Hotkey + Clipboard + Conversion
- [ ] 3.3 Test in 5+ applications: Notepad, Word, VS Code, Chrome, Discord
- [ ] 3.4 Test on Windows 10 and Windows 11
- [ ] 3.5 Run for 1 hour continuous use - check for crashes/memory leaks
- [ ] 3.6 Scan final .exe with Windows Defender and verify clean

**Acceptance Criteria:**
- ✅ All unit tests pass
- ✅ Integration tests pass in all tested apps
- ✅ No crashes or memory leaks in 1-hour test
- ✅ No antivirus warnings on final .exe

## Phase 4: Documentation & Release (Day 5)

- [ ] 4.1 Write README.md with usage instructions
- [ ] 4.2 Create quick-start guide with screenshots
- [ ] 4.3 Create "config.json guide" for customization
- [ ] 4.4 Compile Release build
- [ ] 4.5 Create installer (optional, for convenience)
- [ ] 4.6 Final verification: Run as standard user, works correctly

**Acceptance Criteria:**
- ✅ README explains how to use in < 2 minutes
- ✅ User can customize hotkey by reading guide
- ✅ Release .exe is < 5MB
- ✅ Works without installation or admin rights

---

## MVP Feature Set

### ✅ Included in MVP
- Global hotkey trigger (Ctrl+Shift+Space)
- Clipboard-based conversion
- Thai↔English character mapping (100+ mappings)
- Floating notification window
- Auto-detect conversion direction
- < 100ms latency
- Works in all applications
- Simple config.json

### ❌ NOT in MVP (Future Releases)
- Word prediction/suggestions
- Dictionary lookup
- Cloud sync
- Multi-language support
- Custom keyboard layout creation
- IME integration

---

## Success Metrics

| Metric | Target | Verification |
|--------|--------|---|
| **Time to MVP** | 2-3 days | Complete all Phase 1 tasks |
| **Conversion Accuracy** | > 95% | 100+ test cases pass |
| **Application Support** | 100% | Works in Notepad, Word, VS Code, Chrome, Discord |
| **Latency** | < 100ms | Measure with stopwatch/logging |
| **Memory Usage** | < 10MB | Task Manager monitoring |
| **CPU Usage** | < 1% idle | No background processes |
| **Antivirus Score** | 0 detections | Scan with Windows Defender |
| **Installation** | 0 steps | Just run .exe |

---

## Risk Mitigation

### Risk 1: Clipboard conflicts with other apps
- **Mitigation:** Use proper clipboard ownership/recovery
- **Fallback:** Manual paste if auto-paste fails

### Risk 2: Hotkey conflicts with other applications
- **Mitigation:** Make hotkey user-configurable
- **Fallback:** Use different default (e.g., Alt+Shift+C)

### Risk 3: Special character handling
- **Mitigation:** Test with 50+ Thai-English character pairs
- **Fallback:** Keep unmapped characters unchanged

### Risk 4: Performance degradation
- **Mitigation:** Profiling and optimization
- **Fallback:** Simplify conversion algorithm if needed

---

## Parallel Work Opportunities

- [ ] 3.1 (Unit Tests) can start in parallel with 2.x (Polish)
- [ ] 4.x (Documentation) can start in parallel with 3.x (Testing)

---

## Success Criteria Summary

**MVP READY when:**
- ✅ Hotkey triggers reliably
- ✅ Conversion accuracy > 95%
- ✅ Works in all major applications
- ✅ No crashes or memory leaks
- ✅ Zero antivirus warnings
- ✅ User can start using immediately (Day 3-4)

**PRODUCTION READY when:**
- ✅ All tests pass
- ✅ 24-hour stability test complete
- ✅ Documentation complete
- ✅ Installer (optional) tested
- ✅ Ready to share with users (Day 5)
