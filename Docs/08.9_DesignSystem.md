# Design System: EduPulse School ERP SaaS (Stripe/Linear Redesign)

**Product Name:** EduPulse School ERP SaaS  
**Visual Identity:** Clean, Premium, Minimalist, High-Trust (Stripe / Linear / Notion-style)  
**Target Users:** School Principals, Administrators, Teachers, Parents  
**Document Status:** Approved & Mandatory for all FrontEnd Pages  

This document defines the visual layout rules, CSS custom tokens, components specifications, and responsive behaviors mandatory for all screens (Students, Staff, Attendance, Fees, Exams, Reports, Settings).

---

## 1. Color Palette Tokens

```css
:root {
  /* Brand Colors */
  --color-primary: #6366f1;            /* Stripe/Linear Modern Indigo */
  --color-primary-hover: #4f46e5;
  --color-primary-light: #eef2ff;      /* Active state background tint */
  --color-secondary: #0f172a;          /* Slate 900 */
  --color-secondary-hover: #1e293b;
  
  /* Layout Canvas Backgrounds */
  --color-bg-main: #f8fafc;            /* Clean light slate backdrop */
  --color-bg-surface: #ffffff;         /* Card background */
  --color-bg-sidebar: #ffffff;         /* Pure white navigation panels */
  
  /* Borders (Soft & Non-Intrusive) */
  --color-border-subtle: #f1f5f9;      /* Slate 100 */
  --color-border-default: #e2e8f0;     /* Slate 200 */
  --color-border-focus: #c7d2fe;       /* Indigo 200 */
  
  /* Typography Colors */
  --color-text-primary: #0f172a;       /* Slate 900 */
  --color-text-secondary: #475569;     /* Slate 600 */
  --color-text-muted: #94a3b8;         /* Slate 400 */
  --color-text-light: #334155;         /* Slate 700 */

  /* Semantic State Colors */
  --color-success: #10b981;
  --color-success-bg: #f0fdf4;
  --color-warning: #f59e0b;
  --color-warning-bg: #fefbeb;
  --color-danger: #ef4444;
  --color-danger-bg: #fef2f2;
  --color-info: #3b82f6;
  --color-info-bg: #eff6ff;

  /* Attendance Grid Colors */
  --color-attendance-present: #10b981;
  --color-attendance-present-bg: #f0fdf4;
  --color-attendance-absent: #f43f5e;
  --color-attendance-absent-bg: #fff1f2;
  --color-attendance-late: #f59e0b;
  --color-attendance-late-bg: #fffbeb;
  --color-attendance-halfday: #6366f1;
  --color-attendance-halfday-bg: #eef2ff;
}
```

---

## 2. Typography Scale

* **Font Family:** `Inter, system-ui, -apple-system, sans-serif` (Premium readability)
* **Visual Weights:**
  * Regular: `400`
  * Medium: `500` (Labels, Navigation sub-links)
  * Semibold: `600` (Card titles, Table headers)
  * Bold: `700` (KPI stats, Page headers)
* **Scale Sizes:**
  * `var(--font-size-xs)`: `12px` (Log timestamps, uppercase table headers, labels)
  * `var(--font-size-sm)`: `14px` (Main content, table cell value text)
  * `var(--font-size-md)`: `16px` (Card subheadings, buttons text)
  * `var(--font-size-lg)`: `18px` (Widget titles, alert headers)
  * `var(--font-size-xl)`: `20px` (Dashboard sections titles)
  * `var(--font-size-2xl)`: `24px` (Modal headers, main page titles)
  * `var(--font-size-3xl)`: `32px` (KPI numeric counts)
  * Metric Highlight: `38px` (`letter-spacing: -0.03em`) for dashboard metrics.

---

## 3. Sidebar Specifications

* **Dimensions:**
  * Expanded State: `260px` width.
  * Collapsed State: `76px` width.
* **Colors:** Background `#ffffff`, border-right `1px solid rgba(226, 232, 240, 0.8)`.
* **State Transition:** `width 0.22s cubic-bezier(0.16, 1, 0.3, 1)`.
* **Link Navigation Styling:**
  * Default: Slate-600 (`var(--color-text-secondary)`), background transparent.
  * Active: Modern Indigo (`var(--color-primary)`), background soft violet tint (`var(--color-primary-light)`), font weight `600`, rounded corners `--radius-sm` (`10px`).
  * Hover: Slate-800 (`var(--color-secondary-hover)`), background `#f8fafc`.
* **Collapsed Behavior:** 
  * Hide text tags (using class `.sidebar-nav-text`, `.sidebar-brand-text`, `.sidebar-footer-text`).
  * Center navigation icons (using class `.sidebar-nav-icon` with `width: 100%; text-align: center; margin-right: 0`).

---

## 4. Header Specifications

* **Dimensions:** Height `70px`, sticky positioning at the top of the canvas, `z-index: 100`.
* **Colors:** Background `#ffffff`, bottom-border `1px solid rgba(226, 232, 240, 0.6)`.
* **Visual Components:**
  * **Sidebar Toggle Button:** Light icon button (`38px` square, background `#fafafa`, radius `6px`) placed left-aligned to trigger sidebar collapse state.
  * **School Identity Details:** Mapped dynamically (displays *EduPulse International Academy* with code *EPA-2026*).
  * **Academic Year Dropdown Selector:** Clean background dropdown context button.
  * **Utility Actions (Far Right):** 🔔 Notifications bell icon (with red pending indicator counts badge), ⚙️ Settings cog, and 👤 Profile dropdown actions.

---

## 5. Card Specifications

* **Grid Cards (`.ep-card`):**
  * Border: `1px solid rgba(226, 232, 240, 0.8)` (Thin borders, no heavy dark frames).
  * Corner Rounding: `--radius-lg` (`18px`) for a premium modern aesthetic.
  * Shadows: `--shadow-sm` (`0 1px 2px rgba(15, 23, 42, 0.03)`).
  * Hover state: Slight raise translation (`translateY(-2px)`) with elevation shadow (`--shadow-lg` / `box-shadow: 0 12px 24px -4px rgba(15, 23, 42, 0.04)`).
* **Metric Cards (`.ep-stat-card`):**
  * Padding: `--space-xl` (`32px`), layout flex spacing.
  * Icon badge: Background soft primary tint at 8% opacity.

---

## 6. Table Specifications

* **Table Roster Frame (`.ep-table-container`):** Card-wrapped outer boundaries, using `--radius-lg` (`18px`).
* **Header Row:** Height `44px`, background fill `#fafafa`, text aligned left.
* **Data Row:** Height `52px`, background white. Row highlight transition on hover (`background-color: #fafafa`).
* **Grid Separation:** Bottom border dividers `1px solid var(--color-border-subtle)` (slate-100), no vertical cell lines.
* **Responsive Wrap:** Wrapper must declare `overflow-x: auto` for mobile swipe scrolls.

---

## 7. Form Specifications

* **Form Fields Labels:** Class `.ep-form-label` above inputs. Small bold uppercase typography (`--font-size-xs`, `--font-weight-semibold`, `letter-spacing: 0.05em`).
* **Inputs & Selects (`.ep-input`):**
  * Height: `44px`.
  * Borders: `1px solid var(--color-border-default)`, radius `--radius-xs` (`6px`).
  * Focus State: Border transitions to `--color-border-focus` with an outline halo shadow (`--shadow-ring` / `0 0 0 4px rgba(99, 102, 241, 0.12)`).
  * Validation Error: Border outline changes to `--color-danger` (`#ef4444`).

---

## 8. Modal Specifications

* **Backdrop Layer:** Overlay fill `rgba(15, 23, 42, 0.2)` with a slight blur effect (`backdrop-filter: blur(2px)`).
* **Modal Dialog:** Background `#ffffff`, rounded corners `--radius-lg` (`18px`), elevation shadow `--shadow-xl`.
* **Grid Spacing:** Inner padding `--space-lg` (`24px`). Buttons in the footer are right-aligned.

---

## 9. Button Variants

| Button Variant | Styling Token Rules | Hover & Focus State |
| :--- | :--- | :--- |
| **Primary (Solid)** | Background `--color-primary`, text `#ffffff`, radius `--radius-xs` (`6px`) | Darker indigo background (`--color-primary-hover`), focus shadow-ring |
| **Secondary (Solid)** | Background `--color-secondary` (Slate-900), text `#ffffff` | Slate-800 (`--color-secondary-hover`) background |
| **Outline** | Background transparent, border `1px solid --color-border-default`, text Slate-600 | Light gray backdrop (`#fafafa`), text Slate-900 |
| **Danger (Solid)** | Background `--color-danger` (`#ef4444`), text `#ffffff` | Darker red background |
| **Ghost / Link** | Borderless, text `--color-primary`, font weight `600` | Faint primary light text backdrop tint |

---

## 10. Status Badges

* **Pill Badges (`.ep-badge`):** Rounded pills using `--radius-full`, small bold uppercase copy (`--font-size-xs`, `letter-spacing: 0.05em`).
* **Status Badges Mappings:**
  * **Success / Active / Marked:** Background `--color-success-bg` (`#f0fdf4`), text `--color-success` (`#10b981`).
  * **Warning / Pending:** Background `--color-warning-bg` (`#fefbeb`), text `--color-warning` (`#f59e0b`).
  * **Danger / Suspended / Absent:** Background `--color-danger-bg` (`#fef2f2`), text `--color-danger` (`#ef4444`).

---

## 11. Empty States

* **Visual Center:** Padding `--space-2xl` (`48px`) top and bottom.
* **Component Details:**
  * Icon placeholder: A line icon in `--color-text-muted` at 40% opacity.
  * Heading: Bold title (`--font-size-md`) followed by a short description explaining how to create data.
  * Action CTA: A primary variant button placed directly underneath.

---

## 12. Responsive Breakpoints

* **Mobile Viewports (< 768px):**
  * Sidebar panel hides overlay to the left (`left: -260px`).
  * Toggling the hamburger button adds the class `.mobile-sidebar-open` to the layout wrapper, sliding the menu drawer into view.
  * The background renders a semi-transparent blurred backdrop overlay (`.ep-backdrop`).
  * Form grids stack into single-column layouts.
* **Tablet Viewports (768px to 992px):**
  * Sidebar automatically collapses to the narrow layout (`76px`) by default.
  * KPI widget blocks stack into two-column setups.
* **Desktop Viewports (>= 992px):**
  * Sidebar remains expanded (`260px`) by default unless collapsed by the header toggle button.
  * Tables and charts render side-by-side inside grid containers.
