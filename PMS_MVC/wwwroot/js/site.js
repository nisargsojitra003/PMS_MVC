// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function handleDeleteProductClick() {
    $(document).on('click', '.deleteProduct', function () {
        $("#delProductModal").modal("show");
        $("#ProductIdVal").val($(this).closest('.tr').data('rid'));
        // Add any additional logic here if needed
    });
}

function validateForm() {
    var fileInput = document.getElementById("myFile");
    var file = fileInput.files[0];

    // Check if a file is selected
    if (!file) {
        alert("Please select a file.");
        return false;
    }

    // Check file type
    var allowedTypes = ["image/jpeg", "image/png", "image/jpg"];
    if (!allowedTypes.includes(file.type)) {
        alert("Only JPEG, PNG, and JPG files are allowed.");
        return false;
    }

    // Check file size (example: maximum file size of 5MB)
    var maxSize = 5 * 1024 * 1024; // 5MB in bytes
    if (file.size > maxSize) {
        alert("File size exceeds the maximum limit of 5MB.");
        return false;
    }

    // Additional validation logic can be added here

    return true; // Form submission allowed if all validations pass
}