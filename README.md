# Product Filter System

## Overview
This project is a **Product Filtering System** built in Unity. It allows users to select product categories, view related subcategories dynamically, and apply filters to refine product selection. Users can also clear filters, ensuring a seamless user experience.

## Features
- **Category & Subcategory Selection**: Dynamically show relevant subcategories when selecting a category.
- **Filtering System**: Filter products based on selected categories and subcategories.
- **Reset Filters**: Clear all selected filters and reset the UI.
- **Product Selection**: View product details and navigate back to the product list.

## Flow Diagram
1. **User opens filter panel** ➝ Filter options appear.
2. **User selects a main category** ➝ Relevant subcategories appear.
3. **User selects subcategories** ➝ Products are filtered based on selection.
4. **User applies filters** ➝ Filtered products are displayed.
5. **User selects a product** ➝ Product details are shown.
6. **User navigates back** ➝ Returns to the filtered product list.
7. **User clears filters** ➝ All filters are reset, UI updates.

## Usage
- Click on a **category toggle** to display subcategories.
- Select/deselect subcategories to refine results.
- Click **Apply Filters** to update the product list.
- Click on a **product** to view its details.
- Click **Back** to return to the product list.
- Click **Reset Filters** to clear selections and reset UI.

## Technical Details
- **FilterController.cs** manages the filter UI and interactions.
- **ProductManager.cs** handles product data and filtering logic.
- **Category & Subcategory Toggles** dynamically update based on user selection.

## Notes
- The subcategory toggles are enabled only when a main category is selected.
- Clearing filters resets all toggles and hides subcategories.

## Contact
For any inquiries, feel free to reach out.

---
**Author:** Ajay Uthaman  

