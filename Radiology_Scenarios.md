# Use Case Scenarios: Radiology Management & Doctor Referrals

This document outlines the key use cases for the **Radiology Management** and **Doctor Referral** modules within the **Asia Hospital Information System (HIS)**. These scenarios cover the end-to-end flow from administrative setup to clinical execution.

---

## 1. Radiology Master Data Management
**Actor:** System Administrator / Radiology Manager  
**Goal:** Define and manage the catalog of radiology exams and their associated prices.

### Scenario 1.1: Define a New Radiology Exam
1.  **Login**: User logs in with `Admin` or `RadiologyManager` role.
2.  **Navigation**: Navigates to **Definitions > Radiology**.
3.  **Action**: Clicks the **"New Exam"** button.
4.  **Input**:
    *   **Name**: Enters "Chest X-Ray (PA View)".
    *   **Code**: System auto-generates `RAD-001` (or allows manual override if configured).
    *   **Modality**: Selects "X-Ray".
    *   **Body Part**: Enters "Chest".
    *   **Price**: Enters `150.00`.
    *   **Instructions**: Enters "Remove metal objects/jewelry from chest area."
    *   **Active**: Checks the "Is Active" box.
5.  **Save**: Clicks **Save**.
6.  **Outcome**: The new exam appears in the list. It is now available for doctors to order.

### Scenario 1.2: Update Exam Price
1.  **Search**: User searches for "Chest X-Ray" in the Radiology Master Data screen.
2.  **Action**: Clicks the **Edit** (pencil) icon next to the exam.
3.  **Input**: Updates the **Price** from `150.00` to `175.00`.
4.  **Save**: Clicks **Save**.
5.  **Outcome**: The price is updated immediately for all *new* orders. Existing orders retain their original price.

---

## 2. Doctor Referral (Ordering Radiology)
**Actor:** Doctor  
**Goal:** Order a radiology exam for a patient during a consultation.

### Scenario 2.1: Order an X-Ray for a Patient
1.  **Context**: Doctor is viewing a patient's medical record or is in an active consultation session.
2.  **Action**: Clicks **"Add Order"** or **"New Referral"**.
3.  **Selection**: Selects **"Radiology"** as the order type.
4.  **Search**: Types "Chest" in the search bar.
5.  **Result**: System displays "Chest X-Ray (PA View)" with price `175.00`.
6.  **Add**: Doctor adds the item to the order list.
7.  **Clinical Notes**: Doctor adds a note: "Suspected pneumonia, check for consolidation."
8.  **Submit**: Clicks **"Send Referral"**.
9.  **Outcome**:
    *   A **Radiology Order** is created with status `Pending`.
    *   The patient's billing account is charged `175.00` (depending on insurance/payment flow).
    *   The Radiology Department sees the new request in their worklist.

---

## 3. Radiology Fulfillment
**Actor:** Radiologist / Technician  
**Goal:** Perform the exam and enter results.

### Scenario 3.1: Receive and Process Order
1.  **Login**: User logs in with `RadiologyTechnician` role.
2.  **Navigation**: Navigates to **Radiology > Requests** (Worklist).
3.  **View**: Sees a pending request for "Chest X-Ray" for the patient.
4.  **Action**: Clicks **"Start Exam"** or **"Check-In"**.
    *   *System Status Change*: Status updates to `In Progress`.
5.  **Execution**: Technician performs the X-Ray.
6.  **Complete**: Clicks **"Complete"**.
    *   *System Status Change*: Status updates to `Completed`.

### Scenario 3.2: Enter Results (Reporting)
1.  **Context**: Exam is marked as `Completed`.
2.  **Actor**: Radiologist (Doctor).
3.  **Action**: Opens the request and clicks **"Write Report"**.
4.  **Input**: Types the findings: "Clear lung fields. No sign of consolidation or pneumothorax."
5.  **Upload**: (Optional) Uploads the DICOM image or a snapshot.
6.  **Finalize**: Clicks **"Approve & Send"**.
    *   *System Status Change*: Status updates to `Reported`.
7.  **Outcome**:
    *   The referring doctor receives a notification.
    *   The report is visible in the Patient's Medical Record.

---

## 4. Billing Integration
**Actor:** Receptionist / Billing Officer  
**Goal:** Ensure the service is paid for.

### Scenario 4.1: Cash Payment for Radiology
1.  **Context**: Patient has a Radiology Order but the status is `Unpaid` (if payment is required upfront).
2.  **Navigation**: Receptionist navigates to **Billing > Invoices**.
3.  **Action**: Selects the pending invoice for the Radiology Order.
4.  **Payment**: Collects cash and marks the invoice as **Paid**.
5.  **Trigger**: The Radiology module is notified that the order is "Clear to Proceed" (if workflow requires payment first).

---

## 5. Verification & Testing

You can use the automated tests we set up to verify the **Radiology** module. There are three ways to test:

### 5.1 Frontend UI Tests (Playwright - TypeScript)
**Goal**: Verify the screen looks correct and the "New Exam" modal opens/validates inputs.
*   **File**: `src/angular/e2e/radiology.spec.ts`
*   **Command**:
    ```bash
    cd src/angular
    npm run e2e
    ```
    *This runs the test in a browser (headless by default).*

### 5.2 Backend API Tests (Playwright - TypeScript)
**Goal**: Verify the backend performs CRUD (Create, Read, Delete) correctly without using the UI.
*   **File**: `src/angular/e2e/radiology-api.spec.ts`
*   **Command**:
    ```bash
    cd src/angular
    npm run e2e:api
    ```

### 5.3 Backend E2E Tests (C# .NET)
**Goal**: Run End-to-End tests using C# if you prefer the .NET ecosystem.
*   **File**: `src/test/HIS.E2E.Tests/RadiologyTests.cs`
*   **Command**:
    ```bash
    dotnet test src/test/HIS.E2E.Tests
    ```
