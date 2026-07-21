# Report 2: Software Requirement Specification — **Part 2 of 3**

# 2. Use Case Specifications

> 42 use case specifications. Business rules referenced as `BR-n` and application messages as `MSG-n` are defined in Part 3, Sections 5.1 and 5.2.

## 2.1 Authentication

### 2.1.1 Register Account

| Field | Content |
|---|---|
| **ID and Name** | UC-01 – Register Account |
| **Created By** | NhatNM |
| **Primary Actor** | Guest |
| **Secondary Actors** | Gmail SMTP |
| **Description** | A Guest creates a new PrintHub account in order to place orders, maintain a document library, and use the wallet. |
| **Triggers** | TRIG-1. Guest selects "Register" on the landing page or login screen. |
| **Preconditions** | PRE-1. The Guest does not have an existing account with the submitted email.<br>PRE-2. The Guest has access to the internet and the web client. |
| **Postconditions** | POST-1. A new account is created with the Customer role and a zero wallet balance.<br>POST-2. A verification email is dispatched to the submitted address. |
| **Normal Flow** | 1. The Guest navigates to the registration page.<br>2. The Guest enters full name, email, phone number, password, and password confirmation.<br>3. The System validates the format of all fields.<br>4. The System verifies that the email is not already registered.<br>5. The System hashes the password using BCrypt and creates the account with role Customer.<br>6. The System requests Gmail SMTP to send a verification email.<br>7. The System displays a confirmation message → MSG-4. |
| **Alternative Flows** | A1: Guest registers from the checkout redirect.<br>A1-1. The Guest attempts to place an order without an account and is redirected to registration.<br>A1-2. On successful registration the System returns the Guest to the interrupted order builder with the configured items preserved. |
| **Exceptions** | E1: Email already registered.<br>E1-1. The System detects the email is linked to an existing account.<br>E1-2. The System displays → MSG-2 and offers login or password reset.<br>E2: Password does not meet strength requirements.<br>E2-1. The System detects the password violates BR-4.<br>E2-2. The System displays → MSG-3.<br>E3: Password confirmation does not match.<br>E3-1. The System displays → MSG-5 and does not submit the form. |
| **Priority** | High |
| **Frequency of Use** | Once per user. |
| **Business Rules** | BR-1, BR-2, BR-3, BR-4 |
| **Other Information** | Password strength requirements are displayed during entry. The account is usable immediately; email verification is required before the first wallet top-up. |
| **Assumptions** | ASM-1. Each email address maps to exactly one account.<br>ASM-2. The verification link expires after a defined period. |

### 2.1.2 Log In

| Field | Content |
|---|---|
| **ID and Name** | UC-02 – Log In |
| **Created By** | NhatNM |
| **Primary Actor** | Guest |
| **Secondary Actors** | None |
| **Description** | A registered user authenticates and receives a JWT access token and a refresh token granting access to role-appropriate functions. |
| **Triggers** | TRIG-1. Guest selects "Log In" on the web client, or opens the desktop client. |
| **Preconditions** | PRE-1. The user has a registered account.<br>PRE-2. The account status is Active, not Locked. |
| **Postconditions** | POST-1. The user is authenticated; an access token containing user identifier, role, and shop membership claims is issued.<br>POST-2. A refresh token is persisted server-side. |
| **Normal Flow** | 1. The user navigates to the login screen.<br>2. The user enters email and password.<br>3. The System retrieves the account and verifies the password against the stored BCrypt hash.<br>4. The System verifies that the account status is Active.<br>5. The System issues an access token and a refresh token.<br>6. The System redirects the user to the home screen appropriate to their role. |
| **Alternative Flows** | A1: Login through the desktop client.<br>A1-1. The user enters credentials in the WPF login window.<br>A1-2. The System verifies that the account role is Shop Owner, Shop Staff, or Admin.<br>A1-3. The System routes the user to the Shop Dashboard or Admin Dashboard accordingly.<br>A2: Access token expired during a session.<br>A2-1. The client submits the refresh token.<br>A2-2. The System validates the refresh token and issues a new access token without requiring re-entry of credentials. |
| **Exceptions** | E1: Incorrect email or password.<br>E1-1. The System displays a generic error → MSG-1 without indicating which field is wrong.<br>E2: Account is locked.<br>E2-1. The System displays → MSG-6 and does not issue a token.<br>E3: Customer attempts to log in to the desktop client.<br>E3-1. The System rejects the login with → MSG-7, since the desktop client serves shop and administrator roles only. |
| **Priority** | High |
| **Frequency of Use** | Frequently — at the start of each session. |
| **Business Rules** | BR-5, BR-6, BR-7 |
| **Other Information** | The error message is deliberately generic so that account existence cannot be probed. Failed attempts are rate-limited. |
| **Assumptions** | ASM-1. Access tokens are short-lived; refresh tokens are long-lived and revocable. |

### 2.1.3 Forgot Password

| Field | Content |
|---|---|
| **ID and Name** | UC-03 – Forgot Password |
| **Created By** | NhatNM |
| **Primary Actor** | Guest |
| **Secondary Actors** | Gmail SMTP |
| **Description** | A user who cannot recall their password requests a reset link delivered to their registered email address. |
| **Triggers** | TRIG-1. User selects "Forgot Password" on the login screen. |
| **Preconditions** | PRE-1. The user has a registered account with a valid email address. |
| **Postconditions** | POST-1. A time-limited reset token is generated and emailed.<br>POST-2. On successful reset, all existing refresh tokens for the account are revoked. |
| **Normal Flow** | 1. The user enters their registered email address.<br>2. The System displays a neutral acknowledgement → MSG-8 regardless of whether the address exists.<br>3. If the address exists, the System generates a single-use reset token with an expiry and emails a reset link.<br>4. The user opens the link and enters a new password and confirmation.<br>5. The System validates the token and the password strength.<br>6. The System updates the password hash, revokes all refresh tokens, and invalidates the reset token.<br>7. The System displays → MSG-9 and redirects to login. |
| **Alternative Flows** | A1: User requests a new link after expiry.<br>A1-1. The user submits the email again and receives a fresh token; any previous token is invalidated. |
| **Exceptions** | E1: Reset token expired or already used.<br>E1-1. The System displays → MSG-10 and offers to send a new link.<br>E2: New password violates strength requirements.<br>E2-1. The System displays → MSG-3. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-4, BR-8, BR-9 |
| **Other Information** | The acknowledgement is deliberately neutral so that account existence cannot be probed through this flow. |
| **Assumptions** | ASM-1. The user retains access to their registered mailbox. |

## 2.2 Profile Management

### 2.2.1 Log Out

| Field | Content |
|---|---|
| **ID and Name** | UC-04 – Log Out |
| **Created By** | NhatNM |
| **Primary Actor** | Common User |
| **Secondary Actors** | None |
| **Description** | An authenticated user terminates their session; the refresh token is revoked server-side so the session cannot be resumed. |
| **Triggers** | TRIG-1. User selects "Log Out". |
| **Preconditions** | PRE-1. The user is currently authenticated. |
| **Postconditions** | POST-1. The refresh token is marked revoked in the database.<br>POST-2. Client-side tokens are cleared and the user is returned to the public landing page or login window. |
| **Normal Flow** | 1. The user selects "Log Out".<br>2. The System displays a confirmation dialog.<br>3. The user confirms.<br>4. The System revokes the refresh token server-side.<br>5. The System records the logout event in the audit log.<br>6. The client clears stored tokens and redirects. |
| **Alternative Flows** | A1: User cancels the confirmation dialog and remains logged in.<br>A2: Automatic logout on refresh token expiry — the client detects the failed refresh and performs the same client-side cleanup. |
| **Exceptions** | E1: Network failure during revocation.<br>E1-1. The client clears local tokens regardless and displays → MSG-11 advising that the session may remain active until expiry. |
| **Priority** | High |
| **Frequency of Use** | Frequently. |
| **Business Rules** | BR-10, BR-11 |
| **Other Information** | Client-side token removal alone is not sufficient; server-side revocation is mandatory. |
| **Assumptions** | ASM-1. Access tokens are short-lived, bounding the window in which a revoked session remains usable. |

### 2.2.2 Change Password

| Field | Content |
|---|---|
| **ID and Name** | UC-05 – Change Password |
| **Created By** | NhatNM |
| **Primary Actor** | Common User |
| **Secondary Actors** | None |
| **Description** | An authenticated user updates their password after verifying the current one. |
| **Triggers** | TRIG-1. User selects "Change Password" from the profile screen. |
| **Preconditions** | PRE-1. The user is authenticated. |
| **Postconditions** | POST-1. The stored password hash is replaced.<br>POST-2. All refresh tokens for the account are revoked, forcing re-authentication on other devices. |
| **Normal Flow** | 1. The user opens the change password screen.<br>2. The user enters the current password, the new password, and confirmation.<br>3. The System verifies the current password against the stored hash.<br>4. The System validates the new password against strength rules and confirms it differs from the current one.<br>5. The System stores the new hash and revokes all refresh tokens.<br>6. The System records the event in the audit log and displays → MSG-12. |
| **Alternative Flows** | None. |
| **Exceptions** | E1: Current password incorrect.<br>E1-1. The System displays → MSG-13 and does not change the password.<br>E2: New password identical to current.<br>E2-1. The System displays → MSG-14.<br>E3: New password fails strength rules.<br>E3-1. The System displays → MSG-3. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-4, BR-9, BR-11, BR-12 |
| **Other Information** | Password fields are never returned by any API, even in masked form. |
| **Assumptions** | ASM-1. The user knows their current password; otherwise UC-03 applies. |

### 2.2.3 View Personal Profile

| Field | Content |
|---|---|
| **ID and Name** | UC-06 – View Personal Profile |
| **Created By** | NhatNM |
| **Primary Actor** | Common User |
| **Secondary Actors** | None |
| **Description** | An authenticated user views their own account information. |
| **Triggers** | TRIG-1. User opens the profile screen. |
| **Preconditions** | PRE-1. The user is authenticated. |
| **Postconditions** | POST-1. The user's own profile data is displayed. |
| **Normal Flow** | 1. The user opens the profile screen.<br>2. The System resolves the user identity from the access token, ignoring any identifier supplied by the client.<br>3. The System retrieves and displays full name, email, phone number, default address, role, wallet balance, and registration date. |
| **Alternative Flows** | A1: Shop Owner or Shop Staff views the profile in the desktop client; the System additionally displays the shop membership and position. |
| **Exceptions** | E1: Profile could not be loaded.<br>E1-1. The System displays → MSG-15 with a retry option. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-13, BR-14 |
| **Other Information** | The password hash is never included in the response. |
| **Assumptions** | ASM-1. Identity is always derived from the token, never from a client-supplied parameter. |

### 2.2.4 Update Personal Profile

| Field | Content |
|---|---|
| **ID and Name** | UC-07 – Update Personal Profile |
| **Created By** | NhatNM |
| **Primary Actor** | Common User |
| **Secondary Actors** | Routing Service |
| **Description** | An authenticated user edits their display name, phone number, and default address. |
| **Triggers** | TRIG-1. User selects "Edit" on the profile screen. |
| **Preconditions** | PRE-1. The user is authenticated. |
| **Postconditions** | POST-1. The updated profile is persisted and the change is recorded in the audit log. |
| **Normal Flow** | 1. The user opens the edit form pre-filled with current values.<br>2. The user modifies full name, phone number, or default address.<br>3. The System validates field formats and lengths.<br>4. If the address changed, the System resolves coordinates through the Routing Service for use in distance ranking.<br>5. The System persists the changes and records them in the audit log.<br>6. The System displays → MSG-16. |
| **Alternative Flows** | A1: Address could not be resolved to coordinates.<br>A1-1. The System saves the address text and marks coordinates as unresolved; distance ranking falls back to district-level matching. |
| **Exceptions** | E1: Display name outside the permitted length.<br>E1-1. The System displays → MSG-17.<br>E2: Phone number format invalid.<br>E2-1. The System displays → MSG-18. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-15, BR-16, BR-17 |
| **Other Information** | Email address and role cannot be changed through this use case; both require administrator action. |
| **Assumptions** | ASM-1. The Routing Service geocoding endpoint is available; failure degrades gracefully. |

### 2.2.5 View Notifications

| Field | Content |
|---|---|
| **ID and Name** | UC-08 – View Notifications |
| **Created By** | NhatNM |
| **Primary Actor** | Common User |
| **Secondary Actors** | None |
| **Description** | An authenticated user views system notifications relating to their orders, complaints, wallet, and account. |
| **Triggers** | TRIG-1. User opens the notification panel, or selects the unread indicator. |
| **Preconditions** | PRE-1. The user is authenticated. |
| **Postconditions** | POST-1. Displayed notifications are marked as read. |
| **Normal Flow** | 1. The user opens the notification panel.<br>2. The System retrieves notifications addressed to the authenticated user, ordered newest first, paginated.<br>3. The System displays each notification with its title, content, type, and timestamp.<br>4. The System marks the displayed notifications as read. |
| **Alternative Flows** | A1: User selects a notification linked to an order.<br>A1-1. The System navigates to the corresponding order tracking screen.<br>A2: User filters by unread only. |
| **Exceptions** | E1: No notifications exist.<br>E1-1. The System displays → MSG-19.<br>E2: Notifications could not be loaded.<br>E2-1. The System displays → MSG-20 with a retry option. |
| **Priority** | Medium |
| **Frequency of Use** | Frequently. |
| **Business Rules** | BR-18, BR-19, BR-20 |
| **Other Information** | Notifications are created by an event consumer rather than synchronously within the originating operation. |
| **Assumptions** | ASM-1. A user sees only notifications addressed to their own account. |

## 2.3 Shop Discovery

### 2.3.1 Browse & Search Shops

| Field | Content |
|---|---|
| **ID and Name** | UC-09 – Browse & Search Shops |
| **Created By** | NhatNM |
| **Primary Actor** | Guest, Customer |
| **Secondary Actors** | Routing Service |
| **Description** | A visitor browses the shop directory with filtering, sorting, and pagination in order to identify candidate shops. |
| **Triggers** | TRIG-1. Visitor opens the shop search page or submits a search term. |
| **Preconditions** | None. This use case is available without authentication. |
| **Postconditions** | POST-1. A paginated list of matching Active shops is displayed. |
| **Normal Flow** | 1. The visitor opens the shop search page.<br>2. The visitor optionally enters a keyword and applies filters: service group, service type, district, minimum rating, and price range.<br>3. The visitor selects a sort order: distance, rating, price, or name.<br>4. The System queries shops with status Active, applying the filters and pagination.<br>5. If the visitor's coordinates are known, the System requests distances from the Routing Service.<br>6. The System returns the paginated result with shop name, district, offered service groups, rating, review count, price indicator, and distance. |
| **Alternative Flows** | A1: Authenticated Customer searches.<br>A1-1. The System uses the customer's saved default coordinates for distance computation without requiring manual entry.<br>A2: Query submitted through the OData endpoint.<br>A2-1. The client supplies `$filter`, `$orderby`, `$top`, `$skip`, `$select`, or `$expand`.<br>A2-2. The System applies a mandatory server-side filter restricting results to Active shops before applying the client query. |
| **Exceptions** | E1: No shops match the criteria.<br>E1-1. The System displays → MSG-21 and suggests relaxing the filters.<br>E2: Routing Service unavailable.<br>E2-1. The System returns results without distance values and disables distance sorting, rather than failing the request. |
| **Priority** | High |
| **Frequency of Use** | Very frequently — the primary entry point to the platform. |
| **Business Rules** | BR-21, BR-22, BR-23 |
| **Other Information** | This is one of the endpoints exposed through OData, demonstrating `$filter`, `$orderby`, `$top`, `$skip`, and `$select`. |
| **Assumptions** | ASM-1. Only Active shops are discoverable; Draft, PendingReview, Rejected, and Suspended shops are excluded. |

### 2.3.2 View Shop Detail

| Field | Content |
|---|---|
| **ID and Name** | UC-10 – View Shop Detail |
| **Created By** | NhatNM |
| **Primary Actor** | Guest, Customer |
| **Secondary Actors** | None |
| **Description** | A visitor inspects a shop's profile, published rate card, machine list, and customer reviews before deciding to order. |
| **Triggers** | TRIG-1. Visitor selects a shop from the search results. |
| **Preconditions** | PRE-1. The shop exists and its status is Active. |
| **Postconditions** | POST-1. The shop's public detail is displayed. |
| **Normal Flow** | 1. The visitor selects a shop.<br>2. The System retrieves the shop profile, address, operating hours, and rating.<br>3. The System retrieves the active rate card entries with their unit prices, minimum quantities, and lead times, grouped by service group.<br>4. The System retrieves the machine list showing type and availability.<br>5. The System retrieves reviews, ordered newest first and paginated.<br>6. The System displays the assembled shop detail page. |
| **Alternative Flows** | A1: Authenticated Customer views the page.<br>A1-1. The System additionally displays whether the shop is in the customer's favourites, and offers an "Order from this shop" action.<br>A2: Query submitted through the OData endpoint with `$expand` on rate card entries and reviews. |
| **Exceptions** | E1: Shop not found or not Active.<br>E1-1. The System returns 404 and displays → MSG-22.<br>E2: Shop is Suspended.<br>E2-1. The System displays → MSG-23 indicating the shop is not currently accepting orders. |
| **Priority** | High |
| **Frequency of Use** | Very frequently. |
| **Business Rules** | BR-21, BR-24 |
| **Other Information** | Only rate card entries with `IsActive = true` are shown. Internal fields such as material unit cost are never exposed publicly. |
| **Assumptions** | ASM-1. A shop's published prices are indicative until a quote is computed for a specific order configuration. |

### 2.3.3 Manage Favourite Shops

| Field | Content |
|---|---|
| **ID and Name** | UC-11 – Manage Favourite Shops |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer saves shops to a personal favourites list for quick access on subsequent orders. |
| **Triggers** | TRIG-1. Customer selects the favourite indicator on a shop card or detail page, or opens the favourites list. |
| **Preconditions** | PRE-1. The customer is authenticated.<br>PRE-2. The target shop exists and is Active. |
| **Postconditions** | POST-1. The favourite record is created or removed. |
| **Normal Flow** | 1. The customer selects the favourite indicator on a shop.<br>2. The System creates a favourite record linking the customer and the shop.<br>3. The System updates the indicator state.<br>4. The customer opens the favourites list and the System displays all favourited shops with their current rating and availability. |
| **Alternative Flows** | A1: Customer removes a favourite.<br>A1-1. The System deletes the favourite record and updates the indicator.<br>A2: Customer starts an order directly from the favourites list. |
| **Exceptions** | E1: Shop already favourited.<br>E1-1. The System treats the request as idempotent and returns success without creating a duplicate.<br>E2: Favourites list is empty.<br>E2-1. The System displays → MSG-24. |
| **Priority** | Low |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-25, BR-26 |
| **Other Information** | The pair (CustomerId, ShopId) carries a composite unique constraint. |
| **Assumptions** | ASM-1. A customer sees only their own favourites. |

### 2.3.4 Manage Document Library

| Field | Content |
|---|---|
| **ID and Name** | UC-12 – Manage Document Library |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer uploads, views, renames, and deletes files in a personal document library, so that files can be reused across orders without re-uploading. |
| **Triggers** | TRIG-1. Customer opens the document library, or uploads a file within the order builder. |
| **Preconditions** | PRE-1. The customer is authenticated. |
| **Postconditions** | POST-1. The file is stored server-side and a document record is created, or an existing record is updated or soft-deleted. |
| **Normal Flow** | 1. The customer opens the document library.<br>2. The System displays the customer's own files with name, type, size, declared page count, and upload date, paginated.<br>3. The customer selects a file to upload.<br>4. The System validates the file type against the permitted list and the size against the limit.<br>5. The customer declares the page count and accepts the intellectual property declaration.<br>6. The System stores the file under a server-generated path and creates the document record.<br>7. The System displays → MSG-25. |
| **Alternative Flows** | A1: Customer renames a file.<br>A1-1. The System updates the display name; the stored path is unchanged.<br>A2: Customer deletes a file.<br>A2-1. The System soft-deletes the record and removes it from the library view.<br>A2-2. Files referenced by an order that is not yet Completed cannot be deleted.<br>A3: Customer uploads directly within the order builder; the file is added to the library and immediately attached to the order item. |
| **Exceptions** | E1: File type not permitted.<br>E1-1. The System rejects the upload and displays → MSG-26.<br>E2: File exceeds the size limit.<br>E2-1. The System rejects the upload and displays → MSG-27.<br>E3: Customer attempts to delete a file attached to an active order.<br>E3-1. The System refuses and displays → MSG-28.<br>E4: Intellectual property declaration not accepted.<br>E4-1. The System does not complete the upload and displays → MSG-29. |
| **Priority** | High |
| **Frequency of Use** | Frequently. |
| **Business Rules** | BR-27, BR-28, BR-29, BR-30 |
| **Other Information** | The stored path is never exposed to the client; files are served only through an authorized endpoint that verifies ownership or an active fulfilment relationship. |
| **Assumptions** | ASM-1. Page count is declared by the customer and verified visually by the shop at acceptance (see LI-6). |

## 2.4 Quoting and Ordering

### 2.4.1 Configure Order & Compare Quotes

| Field | Content |
|---|---|
| **ID and Name** | UC-13 – Configure Order & Compare Quotes |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | Quote Engine, Routing Service |
| **Description** | A customer configures one or more order items and receives directly comparable quotes computed at every eligible shop using each shop's own pricing rules. This is the platform's distinguishing capability. |
| **Triggers** | TRIG-1. Customer selects "Compare quotes" after configuring at least one order item. |
| **Preconditions** | PRE-1. The customer is authenticated.<br>PRE-2. At least one order item is configured with a valid service type and quantity. |
| **Postconditions** | POST-1. A quote record is persisted for each eligible shop, each with an expiry timestamp.<br>POST-2. A ranked comparison list is displayed. |
| **Normal Flow** | 1. The customer opens the order builder.<br>2. For each item the customer selects a service type; the System displays only the option fields that service type requires, determined by its pricing model.<br>3. The customer configures the item — copies, page count, paper type, colour mode, and sides for document services; quantity and finishing options for finishing services; material, quality profile, and quantity for fabrication services — and attaches a file where required.<br>4. The customer selects "Compare quotes".<br>5. The System determines eligible shops: status Active, offering every service type present in the order, with a non-offline machine for those service types and sufficient material stock.<br>6. For each eligible shop, the System invokes the Quote Engine over gRPC, passing the item specifications and that shop's rate card and pricing rules.<br>7. The Quote Engine applies the pricing strategy matching each service type's pricing model and returns an itemised breakdown and an estimated completion time.<br>8. The System requests distance and travel time from the Routing Service for each candidate shop.<br>9. The System persists each quote with its breakdown and expiry, and returns the list ranked by the customer's chosen criterion.<br>10. The System displays each result showing total price, estimated completion, distance, and rating, with an expandable price breakdown. |
| **Alternative Flows** | A1: Customer restricts the comparison to favourites or to a single shop.<br>A1-1. The System limits the eligible shop set accordingly and computes quotes only for that subset.<br>A2: Customer changes the ranking criterion.<br>A2-1. The System reorders the already-computed results without recomputing.<br>A3: Customer modifies an item after comparing.<br>A3-1. The System invalidates the existing quotes and requires a new comparison. |
| **Exceptions** | E1: No shop offers every requested service type.<br>E1-1. The System displays → MSG-30, identifying which service types could not be matched, and suggests splitting the order.<br>E2: Quote Engine unavailable.<br>E2-1. The System falls back to the shops' published indicative rates, marks each result as indicative, and displays → MSG-31 stating the price is not final.<br>E3: Routing Service unavailable.<br>E3-1. The System returns results without distance and disables distance ranking.<br>E4: Required option missing for the selected service type.<br>E4-1. The System displays → MSG-32 identifying the missing field. |
| **Priority** | High |
| **Frequency of Use** | Very frequently — invoked on every order. |
| **Business Rules** | BR-31, BR-32, BR-33, BR-34, BR-35 |
| **Other Information** | The stored breakdown records which pricing rules were applied and their effect, so that a disputed price can be explained after the fact. Graceful degradation on Quote Engine failure is deliberate: the platform remains usable when its most computation-heavy component is down. |
| **Assumptions** | ASM-1. Rate card changes do not affect quotes already generated; each quote is evaluated against the rules current at generation time. |

### 2.4.2 Apply Voucher

| Field | Content |
|---|---|
| **ID and Name** | UC-14 – Apply Voucher |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer applies a promotional voucher code to a quoted order and sees the recalculated total before confirming. |
| **Triggers** | TRIG-1. Customer enters a voucher code at checkout. |
| **Preconditions** | PRE-1. The customer has selected a quote and is at checkout.<br>PRE-2. The quote has not expired. |
| **Postconditions** | POST-1. The discount is computed and reflected in the order total; the voucher is associated with the pending order. |
| **Normal Flow** | 1. The customer enters a voucher code at checkout.<br>2. The System retrieves the voucher and verifies it is active and within its validity window.<br>3. The System verifies the order subtotal meets the voucher's minimum order amount.<br>4. The System verifies the voucher's usage limit has not been exhausted and that the customer has not already used it.<br>5. The System computes the discount — a percentage capped at the maximum discount amount, or a fixed amount.<br>6. The System displays the recalculated total and → MSG-33. |
| **Alternative Flows** | A1: Customer removes the applied voucher.<br>A1-1. The System restores the undiscounted total and clears the association. |
| **Exceptions** | E1: Voucher code does not exist or is inactive.<br>E1-1. The System displays → MSG-34.<br>E2: Voucher outside its validity window.<br>E2-1. The System displays → MSG-35.<br>E3: Order subtotal below the minimum.<br>E3-1. The System displays → MSG-36 stating the required minimum.<br>E4: Usage limit exhausted or already used by this customer.<br>E4-1. The System displays → MSG-37. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-36, BR-37, BR-38 |
| **Other Information** | The voucher's usage count is incremented only when the order is successfully placed, not when the code is applied at checkout. |
| **Assumptions** | ASM-1. Only one voucher may be applied per order. |

### 2.4.3 Place Order

| Field | Content |
|---|---|
| **ID and Name** | UC-15 – Place Order |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer confirms a selected quote, chooses a fulfilment method and pickup slot, and pays from the wallet, creating an order awaiting shop acceptance. |
| **Triggers** | TRIG-1. Customer selects "Place order" at checkout. |
| **Preconditions** | PRE-1. The customer is authenticated and has selected a valid, unexpired quote.<br>PRE-2. The wallet balance is at least the order total.<br>PRE-3. The selected shop is still Active. |
| **Postconditions** | POST-1. An order is created in status AwaitingAcceptance with a unique order code.<br>POST-2. The order total is debited from the wallet as a Payment transaction.<br>POST-3. The quote is snapshotted onto the order.<br>POST-4. The shop is notified. |
| **Normal Flow** | 1. The customer reviews the checkout summary: items, selected shop, price breakdown, and discount.<br>2. The customer selects a fulfilment method — pickup or delivery — and, for pickup, a collection slot within the shop's operating hours.<br>3. The customer optionally adds a note to the shop.<br>4. The customer selects "Place order".<br>5. The System verifies that the quote has not expired, that the shop remains Active, and that the wallet balance is sufficient.<br>6. Within a single transaction, the System creates the order and its items, snapshots the quote breakdown onto the order, debits the wallet and writes the Payment transaction, increments the voucher usage count if one was applied, and records the initial status history entry.<br>7. The System publishes a notification event to the shop.<br>8. The System displays the order confirmation with the order code and redirects to order tracking → MSG-38. |
| **Alternative Flows** | A1: Wallet balance insufficient.<br>A1-1. The System offers to top up and returns the customer to checkout with the configuration preserved after top-up completes.<br>A2: Delivery selected.<br>A2-1. The customer supplies or confirms a delivery address; the pickup slot fields are not required. |
| **Exceptions** | E1: Quote expired.<br>E1-1. The System refuses the order and displays → MSG-39, requiring a new comparison.<br>E2: Insufficient wallet balance.<br>E2-1. The System refuses the order and displays → MSG-40.<br>E3: Shop suspended between quoting and confirmation.<br>E3-1. The System refuses the order and displays → MSG-41, suggesting the customer select another shop from the comparison.<br>E4: Pickup slot outside operating hours.<br>E4-1. The System displays → MSG-42. |
| **Priority** | High |
| **Frequency of Use** | Very frequently — the platform's central transaction. |
| **Business Rules** | BR-39, BR-40, BR-41, BR-42, BR-43 |
| **Other Information** | The quote snapshot is the mechanism that makes agreed terms immutable: a subsequent rate card change by the shop cannot alter what the customer agreed to pay. All state changes in step 6 occur within one unit of work. |
| **Assumptions** | ASM-1. Payment is always taken from the wallet; no alternative payment path exists at order time. |

### 2.4.4 Track Order Status

| Field | Content |
|---|---|
| **ID and Name** | UC-16 – Track Order Status |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer views the current status, production progress, and full transition history of one of their own orders. |
| **Triggers** | TRIG-1. Customer opens an order from the order list, a notification, or the confirmation screen. |
| **Preconditions** | PRE-1. The customer is authenticated.<br>PRE-2. The order belongs to the requesting customer. |
| **Postconditions** | POST-1. The order's current state and history are displayed. |
| **Normal Flow** | 1. The customer opens an order.<br>2. The System verifies that the order belongs to the requesting customer.<br>3. The System retrieves the order, its items, the fulfilling shop, the price breakdown, and the status history.<br>4. The System displays a status timeline showing each transition with its timestamp and, where recorded, its reason.<br>5. When the order is InProduction, the System additionally displays the reported progress percentage.<br>6. The System displays the actions available in the current status — cancel, confirm receipt, review, or raise complaint. |
| **Alternative Flows** | A1: Order is InProduction.<br>A1-1. The client polls the status endpoint at a fixed interval so that progress updates without a manual refresh.<br>A2: Order is ReadyForPickup.<br>A2-1. The System displays the shop address, operating hours, and the order code to present at the counter. |
| **Exceptions** | E1: Order belongs to another customer.<br>E1-1. The System returns 403 Forbidden and displays → MSG-43.<br>E2: Order not found.<br>E2-1. The System returns 404 and displays → MSG-44. |
| **Priority** | High |
| **Frequency of Use** | Very frequently. |
| **Business Rules** | BR-13, BR-44, BR-45 |
| **Other Information** | Status history records are append-only and are never modified or deleted, which is what makes them usable as evidence during complaint resolution. |
| **Assumptions** | ASM-1. A customer may view only their own orders regardless of any identifier supplied by the client. |

### 2.4.5 Cancel Order

| Field | Content |
|---|---|
| **ID and Name** | UC-17 – Cancel Order |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer cancels an order before production begins; the refundable amount is determined by the order's status at the time of cancellation. |
| **Triggers** | TRIG-1. Customer selects "Cancel order" on the order tracking screen. |
| **Preconditions** | PRE-1. The customer is authenticated and owns the order.<br>PRE-2. The order status is AwaitingAcceptance or Accepted. |
| **Postconditions** | POST-1. The order moves to Cancelled.<br>POST-2. A Refund transaction credits the computed refundable amount to the wallet.<br>POST-3. The shop is notified and the transition is recorded in history. |
| **Normal Flow** | 1. The customer selects "Cancel order".<br>2. The System verifies ownership and that the current status permits cancellation.<br>3. The System computes the refundable amount: the full total when the status is AwaitingAcceptance, or the total less the cancellation fee when the status is Accepted.<br>4. The System displays a confirmation dialog stating the refundable amount explicitly.<br>5. The customer confirms.<br>6. Within a single transaction, the System transitions the order to Cancelled, writes the Refund transaction, credits the wallet, and appends the status history entry.<br>7. The System notifies the shop and displays → MSG-45 with the refunded amount. |
| **Alternative Flows** | A1: Customer cancels the confirmation dialog; no change occurs.<br>A2: Customer supplies an optional cancellation reason, which is recorded in the status history. |
| **Exceptions** | E1: Order is already InProduction or later.<br>E1-1. The System refuses and displays → MSG-46 explaining that production has begun.<br>E2: Order is already Cancelled, Declined, or Completed.<br>E2-1. The System refuses and displays → MSG-47.<br>E3: Order belongs to another customer.<br>E3-1. The System returns 403 and displays → MSG-43. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-46, BR-47, BR-48 |
| **Other Information** | The cancellation fee on an Accepted order exists because the shop has already committed capacity and possibly prepared materials. The fee is retained by the shop and recorded against it. |
| **Assumptions** | ASM-1. Refunds are always credited to the wallet, never returned to a bank account. |

### 2.4.6 Reorder Previous Order

| Field | Content |
|---|---|
| **ID and Name** | UC-18 – Reorder Previous Order |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | Quote Engine |
| **Description** | A customer creates a new order pre-populated from a previous completed order, avoiding reconfiguration of frequently repeated jobs. |
| **Triggers** | TRIG-1. Customer selects "Reorder" on a past order. |
| **Preconditions** | PRE-1. The customer is authenticated and owns the source order.<br>PRE-2. The source order status is Completed.<br>PRE-3. The referenced files still exist in the document library. |
| **Postconditions** | POST-1. A new draft order is created with the same items and configuration.<br>POST-2. A fresh quote comparison is required before the new order can be placed. |
| **Normal Flow** | 1. The customer selects "Reorder" on a completed order.<br>2. The System verifies ownership and that the source order is Completed.<br>3. The System creates a draft order copying every item's service type, quantity, configuration options, and file reference.<br>4. The System opens the order builder pre-populated with the copied configuration.<br>5. The customer optionally modifies items.<br>6. The customer proceeds to quote comparison (UC-13); prices are recomputed at current rates rather than copied. |
| **Alternative Flows** | A1: A referenced file has been deleted.<br>A1-1. The System copies the remaining items, flags the affected item, and prompts the customer to attach a replacement file.<br>A2: A service type from the source order is no longer offered by the original shop.<br>A2-1. The item is copied and the comparison naturally excludes shops that cannot fulfil it. |
| **Exceptions** | E1: Source order is not Completed.<br>E1-1. The System refuses and displays → MSG-48.<br>E2: All referenced files have been deleted.<br>E2-1. The System refuses and displays → MSG-49. |
| **Priority** | Medium |
| **Frequency of Use** | Frequently for repeat customers. |
| **Business Rules** | BR-49, BR-50 |
| **Other Information** | Prices are deliberately never copied from the source order. A reorder is a new transaction quoted at current rates. |
| **Assumptions** | ASM-1. The customer may select a different shop for the reorder than the original one. |

### 2.4.7 Confirm Order Receipt

| Field | Content |
|---|---|
| **ID and Name** | UC-19 – Confirm Order Receipt |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer confirms that a completed order has been collected, moving it to Completed and enabling review and complaint. |
| **Triggers** | TRIG-1. Customer selects "Confirm receipt" on an order in ReadyForPickup or OutForDelivery. |
| **Preconditions** | PRE-1. The customer is authenticated and owns the order.<br>PRE-2. The order status is ReadyForPickup or OutForDelivery. |
| **Postconditions** | POST-1. The order moves to Completed.<br>POST-2. The platform commission is computed and recorded.<br>POST-3. Review and complaint actions become available. |
| **Normal Flow** | 1. The customer selects "Confirm receipt".<br>2. The System verifies ownership and that the current status permits the transition.<br>3. Within a single transaction, the System transitions the order to Completed, computes the commission from the configured rate, records it against the order, and appends the status history entry.<br>4. The System notifies the shop and displays → MSG-50, prompting the customer to leave a review. |
| **Alternative Flows** | A1: Shop records hand-over at the counter first (UC-34).<br>A1-1. The order is already Completed; the confirm action is not shown to the customer. |
| **Exceptions** | E1: Order not in a collectable status.<br>E1-1. The System refuses and displays → MSG-51.<br>E2: Order belongs to another customer.<br>E2-1. The System returns 403 and displays → MSG-43. |
| **Priority** | High |
| **Frequency of Use** | Once per order. |
| **Business Rules** | BR-51, BR-52 |
| **Other Information** | Either party may complete the order — the customer by confirming receipt, or the shop by recording hand-over — because in practice collection happens at a physical counter. |
| **Assumptions** | ASM-1. Commission is recognised at completion, not at order placement. |

### 2.4.8 View Order History

| Field | Content |
|---|---|
| **ID and Name** | UC-20 – View Order History |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer browses, filters, and searches their own past and current orders. |
| **Triggers** | TRIG-1. Customer opens "My Orders". |
| **Preconditions** | PRE-1. The customer is authenticated. |
| **Postconditions** | POST-1. A paginated list of the customer's own orders is displayed. |
| **Normal Flow** | 1. The customer opens the order list.<br>2. The customer optionally filters by status, shop, service group, or date range, and sorts by date or amount.<br>3. The System applies a mandatory server-side filter restricting results to the authenticated customer's own orders.<br>4. The System returns the paginated result showing order code, shop, item summary, total, status, and placement date. |
| **Alternative Flows** | A1: Query submitted through the OData endpoint.<br>A1-1. The client supplies `$filter`, `$orderby`, `$top`, `$skip`, `$select`, or `$expand` on items and shop.<br>A1-2. The System injects the ownership filter server-side before applying the client query, so a client-supplied filter cannot widen the result set. |
| **Exceptions** | E1: No orders match.<br>E1-1. The System displays → MSG-52. |
| **Priority** | High |
| **Frequency of Use** | Frequently. |
| **Business Rules** | BR-13, BR-53 |
| **Other Information** | This is the second OData endpoint required by the project specification, and the one that demonstrates server-side query scoping most clearly. |
| **Assumptions** | ASM-1. Ownership is derived from the access token, never from a client parameter. |

## 2.5 Wallet

### 2.5.1 Top Up Wallet

| Field | Content |
|---|---|
| **ID and Name** | UC-21 – Top Up Wallet |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | VietQR, Admin |
| **Description** | A customer requests a wallet top-up and receives a VietQR payload carrying a unique reference code to be used in a bank transfer. |
| **Triggers** | TRIG-1. Customer selects "Top up" on the wallet screen, or is prompted by insufficient balance at checkout. |
| **Preconditions** | PRE-1. The customer is authenticated.<br>PRE-2. The account email has been verified. |
| **Postconditions** | POST-1. A wallet transaction is created in Pending status with a unique reference code.<br>POST-2. A VietQR payload is returned for display. |
| **Normal Flow** | 1. The customer enters or selects a top-up amount.<br>2. The System validates the amount against the permitted minimum and maximum.<br>3. The System generates a unique reference code and creates a Pending wallet transaction.<br>4. The System constructs the VietQR payload encoding the platform bank account, the amount, and the reference code.<br>5. The System displays the QR code together with the account details, the amount, and the reference code, and instructs the customer to include the reference code in the transfer description.<br>6. The customer performs the transfer using their banking application.<br>7. An Administrator matches the received transfer to the pending reference code and confirms it.<br>8. The System moves the transaction to Completed, credits the balance, and notifies the customer → MSG-53. |
| **Alternative Flows** | A1: Customer initiates top-up from checkout after an insufficient balance error.<br>A1-1. On confirmation the System returns the customer to checkout with the order configuration preserved.<br>A2: Customer abandons the transfer.<br>A2-1. The pending transaction moves to Expired when its validity window elapses; the balance is unaffected. |
| **Exceptions** | E1: Amount below the minimum or above the maximum.<br>E1-1. The System displays → MSG-54 stating the permitted range.<br>E2: Email not verified.<br>E2-1. The System refuses and displays → MSG-55.<br>E3: Reference code collision.<br>E3-1. The unique constraint rejects the insert and the System regenerates the code transparently. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-54, BR-55, BR-56, BR-57 |
| **Other Information** | Confirmation is performed against the platform record rather than a live banking API (see LI-1). The generated QR is nonetheless a valid VietQR payload and scannable by real banking applications. |
| **Assumptions** | ASM-1. The customer enters the reference code correctly in the transfer description, enabling the Administrator to match it. |

### 2.5.2 View Wallet Statement

| Field | Content |
|---|---|
| **ID and Name** | UC-22 – View Wallet Statement |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer views their current balance and a filterable ledger of every wallet transaction. |
| **Triggers** | TRIG-1. Customer opens the wallet screen. |
| **Preconditions** | PRE-1. The customer is authenticated. |
| **Postconditions** | POST-1. The balance and paginated transaction ledger are displayed. |
| **Normal Flow** | 1. The customer opens the wallet screen.<br>2. The System displays the current balance.<br>3. The System retrieves the customer's own transactions, ordered newest first and paginated.<br>4. The customer optionally filters by transaction type or date range.<br>5. The System displays each transaction with its type, signed amount, resulting balance, related order code where applicable, status, and timestamp. |
| **Alternative Flows** | A1: Customer selects a transaction linked to an order.<br>A1-1. The System navigates to the corresponding order tracking screen.<br>A2: Customer exports the statement; the System serves it as CSV through content negotiation. |
| **Exceptions** | E1: No transactions exist.<br>E1-1. The System displays → MSG-56. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-13, BR-58 |
| **Other Information** | Every transaction records the resulting balance, so the ledger can be verified independently against the stored balance. |
| **Assumptions** | ASM-1. A customer sees only their own transactions. |

## 2.6 Review and Complaint

### 2.6.1 Submit Shop Review

| Field | Content |
|---|---|
| **ID and Name** | UC-23 – Submit Shop Review |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | None |
| **Description** | A customer who has completed an order rates and reviews the fulfilling shop; the rating contributes to the shop's public average used in discovery and comparison. |
| **Triggers** | TRIG-1. Customer selects "Write a review" on a completed order, or follows the prompt shown after confirming receipt. |
| **Preconditions** | PRE-1. The customer is authenticated and owns the order.<br>PRE-2. The order status is Completed.<br>PRE-3. No review already exists for that order. |
| **Postconditions** | POST-1. A review record is created.<br>POST-2. The shop's rating average and review count are recomputed. |
| **Normal Flow** | 1. The customer opens the review form for a completed order.<br>2. The System verifies ownership, that the order is Completed, and that no review exists for it.<br>3. The customer selects a rating from one to five and optionally writes a comment.<br>4. The System validates the rating range and the comment length.<br>5. Within a single transaction, the System creates the review and recomputes the shop's rating average and count.<br>6. The System displays → MSG-57. |
| **Alternative Flows** | A1: Shop replies to the review (UC-35 context).<br>A1-1. The reply is displayed beneath the review on the shop detail page. |
| **Exceptions** | E1: Order is not Completed.<br>E1-1. The System refuses and displays → MSG-58.<br>E2: A review already exists for the order.<br>E2-1. The System refuses and displays → MSG-59.<br>E3: Order belongs to another customer.<br>E3-1. The System returns 403 and displays → MSG-43. |
| **Priority** | Medium |
| **Frequency of Use** | Once per completed order. |
| **Business Rules** | BR-59, BR-60, BR-61 |
| **Other Information** | Only a customer who actually transacted may review, and only once per order. This is enforced by a unique constraint on OrderId, not merely by a UI restriction. |
| **Assumptions** | ASM-1. Reviews are public and are not editable after submission in this phase. |

### 2.6.2 Raise Order Complaint

| Field | Content |
|---|---|
| **ID and Name** | UC-24 – Raise Order Complaint |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | Shop Owner, Shop Staff, Admin |
| **Description** | A customer raises a complaint against a completed order and responds to the shop's proposed resolution, escalating to an Administrator if unsatisfied. |
| **Triggers** | TRIG-1. Customer selects "Report a problem" on a completed order. |
| **Preconditions** | PRE-1. The customer is authenticated and owns the order.<br>PRE-2. The order status is Completed.<br>PRE-3. The complaint window has not elapsed. |
| **Postconditions** | POST-1. A complaint record is created in Open status and routed to the shop.<br>POST-2. On resolution, a refund is credited or a zero-charge replacement order is created. |
| **Normal Flow** | 1. The customer selects a complaint reason and writes a description.<br>2. The System verifies ownership, order status, and that the complaint window has not elapsed.<br>3. The System creates the complaint in Open status and notifies the shop.<br>4. The shop responds with a proposed resolution — reprint or refund (UC-35); the complaint moves to ShopResponded.<br>5. The customer reviews the proposed resolution.<br>6. The customer accepts; the System applies the resolution — crediting a refund to the wallet, or creating a linked zero-charge replacement order — and moves the complaint to Resolved.<br>7. The System displays → MSG-60. |
| **Alternative Flows** | A1: Customer rejects the shop's resolution.<br>A1-1. The complaint moves to Escalated and is routed to an Administrator for adjudication (UC-41).<br>A2: Shop does not respond within the response window.<br>A2-1. The System automatically moves the complaint to Escalated.<br>A3: Customer views their own complaint list and its current status. |
| **Exceptions** | E1: Complaint window elapsed.<br>E1-1. The System refuses and displays → MSG-61 stating the window length.<br>E2: A complaint is already open for this order.<br>E2-1. The System refuses and displays → MSG-62 and links to the existing complaint.<br>E3: Order is not Completed.<br>E3-1. The System refuses and displays → MSG-58. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-62, BR-63, BR-64, BR-65 |
| **Other Information** | The complaint workflow is the platform's mandated dispute mechanism under e-commerce platform obligations (see Report 1, Section 2.5.1). Escalation to the platform, and its finality, are what distinguish a marketplace from a directory. |
| **Assumptions** | ASM-1. Only one open complaint may exist per order at a time. |

## 2.7 Shop Onboarding and Management

### 2.7.1 Apply to Open Shop

| Field | Content |
|---|---|
| **ID and Name** | UC-25 – Apply to Open Shop |
| **Created By** | NhatNM |
| **Primary Actor** | Customer |
| **Secondary Actors** | Admin, Routing Service |
| **Description** | A registered customer submits an application to operate a shop on the platform, which an Administrator subsequently reviews. |
| **Triggers** | TRIG-1. Customer selects "Open a shop" from the customer dashboard. |
| **Preconditions** | PRE-1. The customer is authenticated with a verified email.<br>PRE-2. The customer does not already have a shop in PendingReview or Active status. |
| **Postconditions** | POST-1. A shop record is created in Draft, then moved to PendingReview on submission.<br>POST-2. Administrators are notified of the pending application. |
| **Normal Flow** | 1. The customer opens the shop application form.<br>2. The customer enters shop name, description, address, district, contact number, operating hours, and the service groups they intend to offer.<br>3. The System saves the application in Draft, allowing the customer to return and complete it later.<br>4. The customer submits the application.<br>5. The System validates that all required fields are present and resolves the address to coordinates through the Routing Service.<br>6. The System moves the shop to PendingReview and notifies administrators.<br>7. The System displays → MSG-63 stating that the application is under review. |
| **Alternative Flows** | A1: Application previously rejected.<br>A1-1. The applicant revises the application and resubmits; the shop returns to PendingReview and the previous rejection reason is retained in the record.<br>A2: Applicant saves the draft and returns later; the System restores the saved values. |
| **Exceptions** | E1: Applicant already operates an Active shop.<br>E1-1. The System refuses and displays → MSG-64.<br>E2: An application is already PendingReview.<br>E2-1. The System refuses and displays → MSG-65.<br>E3: Address could not be resolved.<br>E3-1. The System accepts the application and flags coordinates as unresolved for the Administrator to correct at approval. |
| **Priority** | High |
| **Frequency of Use** | Once per shop owner. |
| **Business Rules** | BR-66, BR-67, BR-68 |
| **Other Information** | Administrator approval before a shop may transact is a platform obligation under e-commerce regulation, not a product preference. |
| **Assumptions** | ASM-1. On approval the applicant's role is elevated from Customer to Shop Owner while retaining the ability to order as a customer. |

### 2.7.2 Manage Shop Profile

| Field | Content |
|---|---|
| **ID and Name** | UC-26 – Manage Shop Profile |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Owner |
| **Secondary Actors** | Routing Service |
| **Description** | A shop owner maintains the storefront profile, address, contact details, and operating hours of their own shop. |
| **Triggers** | TRIG-1. Shop Owner opens "Shop Profile" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Shop Owner role.<br>PRE-2. The user owns the shop being edited. |
| **Postconditions** | POST-1. The shop profile is updated and the change recorded in the audit log. |
| **Normal Flow** | 1. The owner opens the shop profile screen, pre-filled with current values.<br>2. The owner modifies name, description, address, contact number, or operating hours.<br>3. The System validates field formats and confirms that the close time is later than the open time.<br>4. If the address changed, the System re-resolves coordinates through the Routing Service.<br>5. The System persists the changes, records them in the audit log, and displays → MSG-16. |
| **Alternative Flows** | A1: Owner temporarily marks the shop as closed.<br>A1-1. The shop remains visible in search but is shown as closed and is excluded from quote comparison until reopened. |
| **Exceptions** | E1: Close time not later than open time.<br>E1-1. The System displays → MSG-66.<br>E2: Owner attempts to edit a shop they do not own.<br>E2-1. The System returns 403 and displays → MSG-67. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-69, BR-70 |
| **Other Information** | Shop status cannot be changed through this use case; suspension and reinstatement are administrator actions. |
| **Assumptions** | ASM-1. Every shop-context operation verifies shop ownership in addition to the role check. |

### 2.7.3 Manage Service & Pricing

| Field | Content |
|---|---|
| **ID and Name** | UC-27 – Manage Service & Pricing |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Owner |
| **Secondary Actors** | None |
| **Description** | A shop owner publishes and maintains the shop's rate card: which service types are offered, at what unit price, with what pricing rules, minimum quantities, and lead times. |
| **Triggers** | TRIG-1. Shop Owner opens "Service & Pricing" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Shop Owner role and owns the shop.<br>PRE-2. The shop status is Active. |
| **Postconditions** | POST-1. Rate card entries and their pricing rules are created, updated, or deactivated.<br>POST-2. Quotes already snapshotted onto confirmed orders are unaffected. |
| **Normal Flow** | 1. The owner opens the rate card screen, showing current entries grouped by service group.<br>2. The owner adds a service type from the platform catalogue.<br>3. The owner sets the unit price, setup fee, minimum quantity, and lead time per unit.<br>4. The owner defines pricing rules for that entry — paper type multipliers, colour mode multipliers, double-sided adjustment, binding surcharges, material rates, quality profile multipliers, and quantity tier discounts.<br>5. The System validates that prices are non-negative, that multipliers are positive, and that quantity tiers do not overlap.<br>6. The System persists the entry and its rules and records the change in the audit log.<br>7. The System displays → MSG-68. |
| **Alternative Flows** | A1: Owner deactivates a service.<br>A1-1. The entry's `IsActive` is set to false; the service disappears from the public rate card and from quote eligibility, but existing orders for it continue unaffected.<br>A2: Owner adjusts a price.<br>A2-1. The new price applies to quotes generated from that moment; orders already confirmed retain their snapshotted price. |
| **Exceptions** | E1: Negative price or non-positive multiplier.<br>E1-1. The System displays → MSG-69.<br>E2: Overlapping quantity tiers.<br>E2-1. The System displays → MSG-70 identifying the conflicting bands.<br>E3: Service type does not exist in the platform catalogue.<br>E3-1. The System displays → MSG-71. |
| **Priority** | High |
| **Frequency of Use** | Occasional, but with high business impact. |
| **Business Rules** | BR-71, BR-72, BR-73, BR-74 |
| **Other Information** | This screen is available to the Shop Owner only. Shop Staff cannot view or modify pricing — the principal reason the two roles are separated. |
| **Assumptions** | ASM-1. The platform service catalogue is maintained centrally by administrators; shops select from it rather than defining arbitrary service types. |

### 2.7.4 Manage Machines & Materials

| Field | Content |
|---|---|
| **ID and Name** | UC-28 – Manage Machines & Materials |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Owner, Shop Staff |
| **Secondary Actors** | None |
| **Description** | Shop personnel maintain the machine registry and material stock levels, and set machine availability status. |
| **Triggers** | TRIG-1. Shop personnel open "Machines & Materials" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Shop Owner or Shop Staff role and belongs to the shop. |
| **Postconditions** | POST-1. Machine and material records are created, updated, or soft-deleted.<br>POST-2. Machine availability and material stock affect quote eligibility from that moment. |
| **Normal Flow** | 1. Shop personnel open the machines and materials screen.<br>2. The System displays the shop's machines with type, service group, and status, and its materials with stock quantity and low-stock threshold.<br>3. Personnel add or edit a machine, setting its name, type, and served service group.<br>4. Personnel set a machine's status to Idle, Maintenance, or Offline.<br>5. Personnel add or adjust material stock quantities and thresholds.<br>6. The System persists the changes and displays → MSG-16. |
| **Alternative Flows** | A1: Machine set to Maintenance or Offline.<br>A1-1. The shop becomes ineligible for quotes on service types served only by that machine, until another machine covers them or the status is restored.<br>A2: Stock falls below the low-stock threshold.<br>A2-1. The System raises a low-stock alert on the shop dashboard and notifies the owner.<br>A3: Owner soft-deletes a machine no longer in service. |
| **Exceptions** | E1: Attempt to set Offline a machine currently assigned to an in-production order.<br>E1-1. The System refuses and displays → MSG-72, identifying the affected order.<br>E2: Negative stock quantity entered.<br>E2-1. The System displays → MSG-73.<br>E3: User does not belong to the shop.<br>E3-1. The System returns 403 and displays → MSG-67. |
| **Priority** | Medium |
| **Frequency of Use** | Frequently — stock is adjusted throughout a working day. |
| **Business Rules** | BR-75, BR-76, BR-77 |
| **Other Information** | Material stock is tracked as an availability indicator and low-stock alert rather than a reserved-quantity ledger; consumption is not deducted automatically per order (a deliberate scope decision). |
| **Assumptions** | ASM-1. Shop Staff may adjust stock and machine status but may not alter pricing. |

### 2.7.5 Manage Shop Staff

| Field | Content |
|---|---|
| **ID and Name** | UC-29 – Manage Shop Staff |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Owner |
| **Secondary Actors** | None |
| **Description** | A shop owner grants, revokes, and lists operational access to their shop for staff members. |
| **Triggers** | TRIG-1. Shop Owner opens "Staff Management" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Shop Owner role and owns the shop.<br>PRE-2. The user to be granted access has a registered platform account. |
| **Postconditions** | POST-1. A staff membership record is created, updated, or deactivated.<br>POST-2. The granted user's role is elevated to Shop Staff and shop membership is added to their token claims on next login. |
| **Normal Flow** | 1. The owner opens the staff management screen showing current staff with position and join date.<br>2. The owner enters the email address of the user to grant access to.<br>3. The System locates the account and verifies it is Active and not already staff of this shop.<br>4. The owner assigns a position label and confirms.<br>5. The System creates the staff membership record, elevates the user's role to Shop Staff, records the action in the audit log, and notifies the user.<br>6. The System displays → MSG-74. |
| **Alternative Flows** | A1: Owner revokes access.<br>A1-1. The System deactivates the membership; the user immediately loses access to the shop's operational screens.<br>A1-2. Orders the staff member previously acted on retain their recorded actor identity in history. |
| **Exceptions** | E1: Email does not correspond to a registered account.<br>E1-1. The System displays → MSG-75 and suggests the person register first.<br>E2: User is already staff of this shop.<br>E2-1. The System displays → MSG-76.<br>E3: Owner attempts to grant access to their own account.<br>E3-1. The System refuses and displays → MSG-77, since the owner already has full access. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-78, BR-79, BR-80 |
| **Other Information** | Staff membership is what makes scoped authorization possible: the access token carries the shop identifiers the user belongs to, and every shop-context operation verifies membership against the record being accessed. |
| **Assumptions** | ASM-1. Revoking access does not delete historical records of actions the staff member performed. |

### 2.7.6 View Shop Revenue Report

| Field | Content |
|---|---|
| **ID and Name** | UC-30 – View Shop Revenue Report |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Owner |
| **Secondary Actors** | None |
| **Description** | A shop owner views revenue, commission, service mix, and machine utilisation reports for their own shop. |
| **Triggers** | TRIG-1. Shop Owner opens "Revenue Report" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Shop Owner role and owns the shop. |
| **Postconditions** | POST-1. Aggregated report data for the requested period is displayed. |
| **Normal Flow** | 1. The owner opens the revenue report screen.<br>2. The owner selects a date range and optionally a grouping — by day, by service group, or by service type.<br>3. The System aggregates completed orders for the owner's shop within the range.<br>4. The System computes gross revenue, platform commission, net revenue, order count, average order value, and cancellation and failure rates.<br>5. The System displays the figures with a chart, and a breakdown by the selected grouping.<br>6. The owner optionally views machine utilisation, showing production time per machine over the period. |
| **Alternative Flows** | A1: Owner exports the report.<br>A1-1. The System serves the report as CSV or XML through content negotiation for use in the shop's own bookkeeping. |
| **Exceptions** | E1: No completed orders in the selected range.<br>E1-1. The System displays → MSG-78 with zero values rather than an error.<br>E2: Owner requests data for a shop they do not own.<br>E2-1. The System returns 403 and displays → MSG-67. |
| **Priority** | Medium |
| **Frequency of Use** | Occasional — typically weekly or monthly. |
| **Business Rules** | BR-81, BR-82 |
| **Other Information** | This screen is restricted to the Shop Owner; Shop Staff cannot view revenue figures. Reports are read-only and computed from completed orders only. |
| **Assumptions** | ASM-1. Revenue is recognised at order completion, consistent with commission recognition. |

## 2.8 Shop Operations

### 2.8.1 View Order Queue

| Field | Content |
|---|---|
| **ID and Name** | UC-31 – View Order Queue |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Staff, Shop Owner |
| **Secondary Actors** | None |
| **Description** | Shop personnel view incoming and in-progress orders for their own shop, forming the operational working list at the counter. |
| **Triggers** | TRIG-1. Shop personnel open the Shop Dashboard in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Shop Owner or Shop Staff role and belongs to the shop. |
| **Postconditions** | POST-1. The shop's order queue is displayed, grouped by status. |
| **Normal Flow** | 1. Shop personnel open the dashboard.<br>2. The System applies a mandatory server-side filter restricting orders to the user's own shop.<br>3. The System retrieves orders in AwaitingAcceptance, Accepted, InProduction, ProductionFailed, and ReadyForPickup, grouped by status.<br>4. The System displays each order with its order code, customer name, item summary, total, requested pickup slot, and elapsed waiting time.<br>5. The System highlights orders approaching or exceeding their estimated completion time.<br>6. The System displays current machine status and any low-stock alerts alongside the queue. |
| **Alternative Flows** | A1: Personnel filter by status or search by order code.<br>A2: Personnel select an order and open the order detail panel (UC-32, UC-33, UC-34).<br>A3: The dashboard refreshes periodically so that newly placed orders appear without manual action. |
| **Exceptions** | E1: No orders in the queue.<br>E1-1. The System displays → MSG-79.<br>E2: User does not belong to any shop.<br>E2-1. The System returns 403 and displays → MSG-67. |
| **Priority** | High |
| **Frequency of Use** | Continuously throughout the working day. |
| **Business Rules** | BR-83, BR-84 |
| **Other Information** | Shop scoping is applied server-side; a client-supplied shop identifier cannot widen the result set to another shop's orders. |
| **Assumptions** | ASM-1. A staff member belonging to more than one shop selects the active shop context on login. |

### 2.8.2 Accept or Decline Order

| Field | Content |
|---|---|
| **ID and Name** | UC-32 – Accept or Decline Order |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Staff, Shop Owner |
| **Secondary Actors** | None |
| **Description** | Shop personnel review an incoming order and either accept it, committing capacity, or decline it with a recorded reason, which triggers an automatic full refund. |
| **Triggers** | TRIG-1. Shop personnel open an order in AwaitingAcceptance from the queue. |
| **Preconditions** | PRE-1. The user is authenticated and belongs to the fulfilling shop.<br>PRE-2. The order status is AwaitingAcceptance. |
| **Postconditions** | POST-1. On acceptance the order moves to Accepted.<br>POST-2. On decline the order moves to Declined and the full total is refunded to the customer's wallet.<br>POST-3. The transition is recorded in history and the customer is notified. |
| **Normal Flow** | 1. Shop personnel open the order detail panel.<br>2. The System displays the item specifications, the attached files for preview and download, the customer note, and the requested pickup slot.<br>3. Personnel inspect the files and verify the specifications are producible.<br>4. Personnel select "Accept".<br>5. The System transitions the order to Accepted, appends the status history entry, and notifies the customer.<br>6. The System displays → MSG-80. |
| **Alternative Flows** | A1: Personnel decline the order.<br>A1-1. Personnel select a decline reason from the defined set — capacity unavailable, material out of stock, file unreadable, specification not supported, copyright concern — and may add a note.<br>A1-2. Within a single transaction, the System transitions the order to Declined, writes a full Refund transaction, credits the customer's wallet, and appends the status history entry with the reason.<br>A1-3. The System notifies the customer with the reason and displays → MSG-81.<br>A2: Personnel download an attached file for closer inspection; the access is recorded. |
| **Exceptions** | E1: Order is no longer in AwaitingAcceptance — for example the customer cancelled first.<br>E1-1. The System refuses and displays → MSG-82 with the current status.<br>E2: Decline attempted without selecting a reason.<br>E2-1. The System refuses and displays → MSG-83.<br>E3: Order belongs to another shop.<br>E3-1. The System returns 403 and displays → MSG-67. |
| **Priority** | High |
| **Frequency of Use** | Very frequently — once per incoming order. |
| **Business Rules** | BR-85, BR-86, BR-87 |
| **Other Information** | A decline reason is mandatory because it is both the customer's explanation and the platform's evidence base for monitoring shop reliability. Refund on decline is automatic and unconditional. |
| **Assumptions** | ASM-1. Declining carries no charge to the customer under any circumstances. |

### 2.8.3 Execute Production

| Field | Content |
|---|---|
| **ID and Name** | UC-33 – Execute Production |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Staff, Shop Owner |
| **Secondary Actors** | Production Agent |
| **Description** | Shop personnel assign a machine and start production; the job is dispatched asynchronously and progress is reported back until completion or failure. |
| **Triggers** | TRIG-1. Shop personnel select "Start production" on an order in Accepted status. |
| **Preconditions** | PRE-1. The user is authenticated and belongs to the fulfilling shop.<br>PRE-2. The order status is Accepted.<br>PRE-3. At least one machine of the required type is available. |
| **Postconditions** | POST-1. A production job message is published to the broker.<br>POST-2. The order moves to InProduction and the machine to Busy.<br>POST-3. On the agent's completion event the order moves to ReadyForPickup or OutForDelivery; on failure it moves to ProductionFailed. |
| **Normal Flow** | 1. Shop personnel open an Accepted order and select "Start production".<br>2. The System lists machines of the required type whose status is Idle.<br>3. Personnel select a machine and confirm.<br>4. The System transitions the order to InProduction, sets the machine to Busy, appends the status history entry, and publishes a production job message containing the order identifier, item specifications, and assigned machine.<br>5. The Production Agent consumes the message and executes the job, publishing progress events at intervals.<br>6. The System consumes progress events, updates the order's progress percentage, and forwards updates to the customer.<br>7. The Agent publishes a completion event; the System transitions the order to ReadyForPickup, or to OutForDelivery when the fulfilment method is delivery, sets the machine back to Idle, and notifies the customer → MSG-84. |
| **Alternative Flows** | A1: Production fails.<br>A1-1. The Agent publishes a failure event with a reason.<br>A1-2. The System transitions the order to ProductionFailed, sets the machine to Idle, and alerts shop personnel.<br>A1-3. Personnel restart production at no charge to the customer, returning the order to InProduction, or decline the order with a full refund.<br>A2: Duplicate progress or completion event delivered.<br>A2-1. The System recognises the order is already in the target state and ignores the event.<br>A3: Message processing fails repeatedly.<br>A3-1. After the configured retry limit the message is routed to the dead letter queue for inspection; the order remains in its last valid state and personnel are alerted. |
| **Exceptions** | E1: No machine of the required type is Idle.<br>E1-1. The System refuses and displays → MSG-85 listing machine statuses.<br>E2: Order is not in Accepted status.<br>E2-1. The System refuses and displays → MSG-82.<br>E3: Message broker unavailable.<br>E3-1. The System does not transition the order, displays → MSG-86, and allows the operation to be retried once connectivity returns. |
| **Priority** | High |
| **Frequency of Use** | Very frequently — once per accepted order. |
| **Business Rules** | BR-88, BR-89, BR-90, BR-91 |
| **Other Information** | Production is asynchronous because a job can occupy a machine for minutes or hours and cannot complete within an HTTP request. Idempotent event handling and the dead letter queue are what make the pipeline safe under redelivery and failure. |
| **Assumptions** | ASM-1. The Production Agent simulates machine behaviour; the hardware boundary is stubbed (see LI-3). |

### 2.8.4 Hand Over Order

| Field | Content |
|---|---|
| **ID and Name** | UC-34 – Hand Over Order |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Staff, Shop Owner |
| **Secondary Actors** | None |
| **Description** | Shop personnel record hand-over of a completed order to the customer at the counter, moving the order to Completed. |
| **Triggers** | TRIG-1. Customer presents an order code at the counter and shop personnel select "Hand over". |
| **Preconditions** | PRE-1. The user is authenticated and belongs to the fulfilling shop.<br>PRE-2. The order status is ReadyForPickup or OutForDelivery. |
| **Postconditions** | POST-1. The order moves to Completed.<br>POST-2. The platform commission is computed and recorded.<br>POST-3. The customer is notified and may then review or complain. |
| **Normal Flow** | 1. Shop personnel enter or scan the order code.<br>2. The System retrieves the order and verifies it belongs to the shop and is in a collectable status.<br>3. The System displays the order summary for verification against the physical output.<br>4. Personnel confirm hand-over.<br>5. Within a single transaction, the System transitions the order to Completed, computes and records the commission, and appends the status history entry.<br>6. The System notifies the customer and displays → MSG-87. |
| **Alternative Flows** | A1: Customer confirms receipt in the web client first (UC-19).<br>A1-1. The order is already Completed and the hand-over action is no longer offered.<br>A2: Personnel search by customer name rather than order code. |
| **Exceptions** | E1: Order code not found.<br>E1-1. The System displays → MSG-88.<br>E2: Order not in a collectable status.<br>E2-1. The System refuses and displays → MSG-51 with the current status.<br>E3: Order belongs to another shop.<br>E3-1. The System returns 403 and displays → MSG-67. |
| **Priority** | High |
| **Frequency of Use** | Very frequently. |
| **Business Rules** | BR-51, BR-52, BR-92 |
| **Other Information** | Either party may complete the order, reflecting that collection occurs at a physical counter where the customer may not have the application open. |
| **Assumptions** | ASM-1. The order code is the reference presented at the counter. |

### 2.8.5 Respond to Complaint

| Field | Content |
|---|---|
| **ID and Name** | UC-35 – Respond to Complaint |
| **Created By** | NhatNM |
| **Primary Actor** | Shop Owner, Shop Staff |
| **Secondary Actors** | Customer |
| **Description** | Shop personnel respond to a customer complaint with a proposed resolution — a free reprint or a refund — and may reply publicly to reviews. |
| **Triggers** | TRIG-1. Shop personnel open a complaint from the complaint list or a notification. |
| **Preconditions** | PRE-1. The user is authenticated and belongs to the shop complained about.<br>PRE-2. The complaint status is Open. |
| **Postconditions** | POST-1. The complaint moves to ShopResponded with a proposed resolution.<br>POST-2. On customer acceptance, a refund is credited or a zero-charge replacement order is created. |
| **Normal Flow** | 1. Shop personnel open the complaint, seeing the order, its items, the reason, and the customer's description.<br>2. Personnel write a response and select a proposed resolution: reprint at no charge, or refund.<br>3. The System records the response and moves the complaint to ShopResponded.<br>4. The System notifies the customer.<br>5. The customer accepts the resolution (UC-24).<br>6. Where the resolution is refund, the System credits the specified amount to the customer's wallet as a Refund transaction; where it is reprint, the System creates a linked replacement order at zero charge in AwaitingAcceptance for the same shop.<br>7. The System moves the complaint to Resolved and displays → MSG-89. |
| **Alternative Flows** | A1: Personnel reject the complaint as unfounded, providing a justification; the complaint still moves to ShopResponded and the customer may accept or escalate.<br>A2: Customer rejects the resolution; the complaint moves to Escalated for administrator adjudication (UC-41).<br>A3: Shop does not respond within the response window; the System escalates the complaint automatically.<br>A4: Shop Owner replies publicly to a review; the reply appears beneath the review on the shop detail page. |
| **Exceptions** | E1: Complaint already Resolved, Escalated, or Closed.<br>E1-1. The System refuses and displays → MSG-90.<br>E2: Proposed refund exceeds the order total.<br>E2-1. The System refuses and displays → MSG-91.<br>E3: Complaint belongs to another shop.<br>E3-1. The System returns 403 and displays → MSG-67. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-93, BR-94, BR-95 |
| **Other Information** | Automatic escalation on shop non-response prevents a shop from resolving a complaint by ignoring it. |
| **Assumptions** | ASM-1. A replacement order created as a reprint resolution follows the normal production workflow but charges nothing. |

## 2.9 Platform Administration

### 2.9.1 Review Shop Application

| Field | Content |
|---|---|
| **ID and Name** | UC-36 – Review Shop Application |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | Shop Owner |
| **Description** | An administrator reviews a pending shop application and approves or rejects it with a recorded reason. |
| **Triggers** | TRIG-1. Administrator opens "Shop Applications" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role.<br>PRE-2. At least one shop is in PendingReview status. |
| **Postconditions** | POST-1. On approval the shop moves to Active and the applicant's role is elevated to Shop Owner.<br>POST-2. On rejection the shop moves to Rejected with a recorded reason.<br>POST-3. The applicant is notified and the decision recorded in the audit log. |
| **Normal Flow** | 1. The administrator opens the pending application list.<br>2. The administrator selects an application and reviews the shop name, address, contact details, and intended service groups.<br>3. The administrator verifies the application against the platform's verification requirements.<br>4. The administrator selects "Approve".<br>5. Within a single transaction, the System transitions the shop to Active, elevates the applicant's role to Shop Owner, records the approving administrator and timestamp, and writes an audit log entry.<br>6. The System notifies the applicant and displays → MSG-92. |
| **Alternative Flows** | A1: Administrator rejects the application.<br>A1-1. The administrator enters a rejection reason, which is mandatory.<br>A1-2. The System transitions the shop to Rejected, stores the reason, notifies the applicant, and writes an audit log entry.<br>A1-3. The applicant may revise and resubmit (UC-25, A1).<br>A2: Administrator corrects unresolved coordinates before approving. |
| **Exceptions** | E1: Application no longer in PendingReview.<br>E1-1. The System refuses and displays → MSG-93 with the current status.<br>E2: Rejection attempted without a reason.<br>E2-1. The System refuses and displays → MSG-94. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-96, BR-97, BR-98 |
| **Other Information** | Verification of sellers before they may transact is a platform obligation under e-commerce regulation. The approval decision is therefore auditable by design. |
| **Assumptions** | ASM-1. Role elevation preserves the applicant's ability to place orders as a customer. |

### 2.9.2 Suspend or Reinstate Shop

| Field | Content |
|---|---|
| **ID and Name** | UC-37 – Suspend or Reinstate Shop |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | Shop Owner |
| **Description** | An administrator suspends an active shop for policy violation, removing it from discovery, and may later reinstate it. |
| **Triggers** | TRIG-1. Administrator selects "Suspend" or "Reinstate" on a shop in the shop management screen. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role.<br>PRE-2. The shop status is Active for suspension, or Suspended for reinstatement. |
| **Postconditions** | POST-1. The shop moves to Suspended or back to Active.<br>POST-2. A suspended shop is excluded from search and quote eligibility, but its in-progress orders continue.<br>POST-3. The action is recorded in the audit log and the owner notified. |
| **Normal Flow** | 1. The administrator opens the shop management screen and selects a shop.<br>2. The administrator selects "Suspend" and enters a mandatory reason.<br>3. The System displays a confirmation dialog stating how many in-progress orders the shop currently holds.<br>4. The administrator confirms.<br>5. The System transitions the shop to Suspended, stores the reason, writes an audit log entry, and notifies the owner → MSG-95.<br>6. The shop disappears from search results and cannot receive new orders; existing orders in Accepted, InProduction, or ReadyForPickup continue to completion. |
| **Alternative Flows** | A1: Administrator reinstates a suspended shop.<br>A1-1. The System transitions the shop to Active, writes an audit log entry, and notifies the owner.<br>A2: Administrator reviews a shop's complaint and failure history before deciding. |
| **Exceptions** | E1: Suspension attempted without a reason.<br>E1-1. The System refuses and displays → MSG-94.<br>E2: Shop is not in a suspendable status.<br>E2-1. The System refuses and displays → MSG-93. |
| **Priority** | Medium |
| **Frequency of Use** | Rare. |
| **Business Rules** | BR-99, BR-100, BR-101 |
| **Other Information** | In-progress orders deliberately continue after suspension so that customers who have already paid are not stranded by an administrative action. |
| **Assumptions** | ASM-1. Suspension is reversible; permanent removal is not implemented in this phase. |

### 2.9.3 Manage User Accounts

| Field | Content |
|---|---|
| **ID and Name** | UC-38 – Manage User Accounts |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Description** | An administrator views, searches, locks, and unlocks platform user accounts. |
| **Triggers** | TRIG-1. Administrator opens "User Management" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role. |
| **Postconditions** | POST-1. The target account's status is updated and the action recorded in the audit log.<br>POST-2. Locking an account revokes all its refresh tokens. |
| **Normal Flow** | 1. The administrator opens the user list.<br>2. The administrator filters by role or status, or searches by name or email.<br>3. The System returns the paginated user list showing name, email, role, status, wallet balance, order count, and registration date.<br>4. The administrator selects a user and reviews their detail.<br>5. The administrator selects "Lock" and enters a mandatory reason.<br>6. The System sets the account status to Locked, revokes all refresh tokens, writes an audit log entry, and notifies the user → MSG-96. |
| **Alternative Flows** | A1: Administrator unlocks an account.<br>A1-1. The System sets the status to Active, writes an audit log entry, and notifies the user.<br>A2: Administrator adjusts a wallet balance to correct an error.<br>A2-1. The System records an Adjustment transaction with a mandatory reason rather than silently editing the balance. |
| **Exceptions** | E1: Administrator attempts to lock their own account.<br>E1-1. The System refuses and displays → MSG-97.<br>E2: Lock attempted without a reason.<br>E2-1. The System refuses and displays → MSG-94.<br>E3: Attempt to lock an owner with a shop holding in-progress orders.<br>E3-1. The System warns and requires explicit confirmation. |
| **Priority** | High |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-102, BR-103, BR-104 |
| **Other Information** | Balance adjustments are always recorded as ledger transactions so that the wallet remains independently verifiable; the balance field is never edited directly. |
| **Assumptions** | ASM-1. A locked account cannot authenticate but its historical records are preserved. |

### 2.9.4 Manage Service Catalog & Commission

| Field | Content |
|---|---|
| **ID and Name** | UC-39 – Manage Service Catalog & Commission |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Description** | An administrator maintains the platform-wide service type catalogue from which shops build their rate cards, and configures the platform commission rate. |
| **Triggers** | TRIG-1. Administrator opens "Service Catalog & Commission" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role. |
| **Postconditions** | POST-1. Service types are created, updated, or deactivated.<br>POST-2. The commission rate is updated; existing completed orders retain their snapshotted rate. |
| **Normal Flow** | 1. The administrator opens the service catalogue screen showing all service types grouped by service group.<br>2. The administrator adds a service type, entering its code, name, service group, pricing model, unit of measure, and whether it requires an attached file.<br>3. The System validates that the code is unique and that the pricing model is one of the three supported strategies.<br>4. The System persists the service type and writes an audit log entry.<br>5. The administrator opens the commission configuration and updates the platform commission rate.<br>6. The System validates the rate is within the permitted range, persists it, and writes an audit log entry → MSG-98. |
| **Alternative Flows** | A1: Administrator deactivates a service type.<br>A1-1. The service type disappears from shop rate card selection and from quote eligibility; existing rate card entries referencing it are deactivated.<br>A1-2. Orders already placed for that service type are unaffected.<br>A2: Administrator reviews which shops currently offer a service type before deactivating it. |
| **Exceptions** | E1: Service type code already exists.<br>E1-1. The System refuses and displays → MSG-99.<br>E2: Commission rate outside the permitted range.<br>E2-1. The System refuses and displays → MSG-100.<br>E3: Attempt to delete a service type referenced by existing orders.<br>E3-1. The System deactivates rather than deletes, preserving referential integrity. |
| **Priority** | Medium |
| **Frequency of Use** | Rare. |
| **Business Rules** | BR-105, BR-106, BR-107 |
| **Other Information** | The pricing model attribute on a service type is what selects the pricing strategy at quote time, so it cannot be changed once orders exist for that service type. |
| **Assumptions** | ASM-1. Commission is snapshotted onto each order at completion, so a rate change never alters historical figures. |

### 2.9.5 Manage Vouchers

| Field | Content |
|---|---|
| **ID and Name** | UC-40 – Manage Vouchers |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Description** | An administrator creates, edits, deactivates, and monitors usage of promotional voucher codes. |
| **Triggers** | TRIG-1. Administrator opens "Voucher Management" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role. |
| **Postconditions** | POST-1. Voucher records are created, updated, or deactivated.<br>POST-2. Redemption counts are visible. |
| **Normal Flow** | 1. The administrator opens the voucher list showing code, discount, validity window, usage limit, redemption count, and status.<br>2. The administrator creates a voucher, entering code, discount type and value, minimum order amount, maximum discount cap, usage limit, and validity window.<br>3. The System validates that the code is unique, the discount value is positive, the percentage does not exceed one hundred, and the validity window is coherent.<br>4. The System persists the voucher and writes an audit log entry → MSG-101.<br>5. The administrator monitors redemption counts against the usage limit. |
| **Alternative Flows** | A1: Administrator deactivates a voucher before its expiry.<br>A1-1. The voucher can no longer be applied; orders that already redeemed it are unaffected.<br>A2: Administrator edits a voucher's validity window or usage limit. |
| **Exceptions** | E1: Voucher code already exists.<br>E1-1. The System refuses and displays → MSG-99.<br>E2: Validity end date before the start date.<br>E2-1. The System refuses and displays → MSG-102.<br>E3: Percentage discount above one hundred, or negative value.<br>E3-1. The System refuses and displays → MSG-103. |
| **Priority** | Low |
| **Frequency of Use** | Occasional. |
| **Business Rules** | BR-108, BR-109 |
| **Other Information** | Redemption count is incremented at successful order placement, so vouchers applied but abandoned at checkout are not consumed. |
| **Assumptions** | ASM-1. Vouchers are platform-funded and apply across all shops in this phase. |

### 2.9.6 Adjudicate Escalated Complaint

| Field | Content |
|---|---|
| **ID and Name** | UC-41 – Adjudicate Escalated Complaint |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | Customer, Shop Owner |
| **Description** | An administrator makes a final ruling on a complaint escalated beyond the shop, either upholding it with a refund or rejecting it. |
| **Triggers** | TRIG-1. Administrator opens an Escalated complaint from the escalation queue. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role.<br>PRE-2. The complaint status is Escalated. |
| **Postconditions** | POST-1. The complaint moves to Closed with a recorded ruling.<br>POST-2. Where the complaint is upheld, a refund is credited to the customer's wallet and charged against the shop. |
| **Normal Flow** | 1. The administrator opens the escalated complaint.<br>2. The System displays the order and its items, the complete order status history, the customer's complaint description, the shop's response if any, and the shop's complaint history.<br>3. The administrator reviews the evidence.<br>4. The administrator selects a ruling: uphold with a refund amount, or reject.<br>5. The administrator enters a mandatory justification.<br>6. Where upheld, within a single transaction the System credits the refund to the customer's wallet, records the amount against the shop, moves the complaint to Closed, and writes an audit log entry.<br>7. The System notifies both parties and displays → MSG-104. |
| **Alternative Flows** | A1: Administrator rejects the complaint.<br>A1-1. The complaint moves to Closed with the justification recorded; no refund is issued and both parties are notified.<br>A2: Administrator upholds the complaint partially, specifying a refund amount lower than the order total. |
| **Exceptions** | E1: Complaint is not in Escalated status.<br>E1-1. The System refuses and displays → MSG-90.<br>E2: Refund amount exceeds the order total.<br>E2-1. The System refuses and displays → MSG-91.<br>E3: Ruling entered without justification.<br>E3-1. The System refuses and displays → MSG-94. |
| **Priority** | High |
| **Frequency of Use** | Rare. |
| **Business Rules** | BR-110, BR-111, BR-112 |
| **Other Information** | Administrator adjudication is final within the system and is what makes the immutable order status history operationally meaningful — the history is the evidence on which the ruling rests. |
| **Assumptions** | ASM-1. The platform absorbs no cost; upheld refunds are recorded against the shop. |

### 2.9.7 View & Export Platform Reports

| Field | Content |
|---|---|
| **ID and Name** | UC-42 – View & Export Platform Reports |
| **Created By** | NhatNM |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Description** | An administrator views platform-wide reports and exports them as JSON, XML, or CSV through content negotiation. |
| **Triggers** | TRIG-1. Administrator opens "Platform Reports" in the desktop client. |
| **Preconditions** | PRE-1. The user is authenticated with the Admin role. |
| **Postconditions** | POST-1. Aggregated platform report data is displayed, and optionally exported in the requested format. |
| **Normal Flow** | 1. The administrator opens the reports screen.<br>2. The administrator selects a report — transaction and revenue summary, shop performance ranking, service mix, or order failure and cancellation analysis — and a date range.<br>3. The System aggregates the data across all shops for the selected range.<br>4. The System displays the figures with supporting charts.<br>5. The administrator selects an export format.<br>6. The System serves the report serialized according to the requested media type — `application/json`, `application/xml`, or `text/csv` — selected by the HTTP `Accept` header. |
| **Alternative Flows** | A1: Administrator drills into a specific shop's figures from the ranking report.<br>A2: Administrator requests XML export for accounting integration, consistent with the electronic invoice format.<br>A3: Administrator requests CSV export for spreadsheet analysis. |
| **Exceptions** | E1: No data in the selected range.<br>E1-1. The System displays → MSG-78 with zero values rather than an error.<br>E2: Requested media type not supported.<br>E2-1. The System returns 406 Not Acceptable and displays → MSG-105.<br>E3: Date range invalid.<br>E3-1. The System refuses and displays → MSG-102. |
| **Priority** | High |
| **Frequency of Use** | Occasional — typically weekly or monthly. |
| **Business Rules** | BR-113, BR-114 |
| **Other Information** | These endpoints are the principal demonstration of content negotiation: the same resource is served as JSON for the desktop client, XML for accounting integration, and CSV for spreadsheet bookkeeping, with the representation selected by the `Accept` header and declared using `[Produces]`. |
| **Assumptions** | ASM-1. Reports are read-only aggregations computed from completed orders; they never modify data. |

---

**Kết thúc Part 2.** Tiếp tục ở `2_SRS_Part3_Requirements.md`.
