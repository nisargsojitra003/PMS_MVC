﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function handleDeleteProductClick() {
    $(document).on('click', '.deleteProduct', function () {
        $("#delProductModal").modal("show");
        $("#ProductIdVal").val($(this).closest('.tr').data('rid'));
        // Add any additional logic here if needed
    });
}

function handleDeleteCategoryClick() {
    $(document).on('click', '.deleteCategory', function () {
        $("#delCategoryModal").modal("show");
        $("#CategoryIdVal").val($(this).closest('.tr').data('rid'));
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

function searchCategory() {

    var searchName = $("#searchCategoryTab").val();
    var searchCode = $("#searchCategoryCodeTab").val();
    var description = $("#searchCategoryDescription").val();
    //console.log(searchName);

    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { searchName: searchName, searchCode: searchCode, description: description },
        type: 'GET',
        success: function (result) {
            //console.log(result);
            $("#categoryListdata").html(result);
        }
    });
}

function searchProcudtName() {

    var searchProduct = $("#searchProduct").val();
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchDescription = $("#searchDescription").val();
    var searchCategory = $("#searchCategory").val();
    //console.log(searchProduct);

    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchProduct: searchProduct, searchCategoryTag: searchCategoryTag, searchDescription: searchDescription, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
            //console.log(result);
            $("#productListdata").html(result);
        }
    });
}

function searchCategoryTag() {
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchProduct = $("#searchProduct").val();
    var searchDescription = $("#searchDescription").val();
    var searchCategory = $("#searchCategory").val();
    //console.log(searchCategoryTag);

    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchCategoryTag: searchCategoryTag, searchProduct: searchProduct, searchDescription: searchDescription, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
            //console.log(result);
            $("#productListdata").html(result);
        }
    });
}

function searchDescription() {
    var searchDescription = $("#searchDescription").val();
    var searchProduct = $("#searchProduct").val();
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchCategory = $("#searchCategory").val();
    console.log(searchDescription);
    console.log(searchProduct);
    console.log(searchCategoryTag);

    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchDescription: searchDescription, searchProduct: searchProduct, searchCategoryTag: searchCategoryTag, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
            //console.log(result);
            $("#productListdata").html(result);
        }
    });
}


function removeProductImage(id) {
    console.log(id);
    let url = '/Product/RemoveProductImage'
    $.ajax({
        url: url,
        data: { id: id },
        success: function (response) {
            window.location.reload();
        }
    });
}

function changeProductpagesize() {
    console.log("hi")
    var catPageSize = $("#prodpagesize").val();
    $.ajax({
        url: "ChangePageSize",
        data: { pageSize: catPageSize },
        success: function (response) {
            if (response.success) {
                $("#productListdata").load("productShared");
                //window.location.reload();
            }
        }
    });
}

function changeProductpage(productPageNumber) {
    console.log(productPageNumber);
    $.ajax({
        url: "ChangePage",
        data: { productPageNumber: productPageNumber },
        success: function (response) {
            if (response.success) {
                $("#productListdata").load("productShared");
                //window.location.reload();
            }
        }
    });
}

