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

//////////////Product Script////////////////////
function changeProductpagesize() {
    console.log("hi")
    var pageSizeProduct = $("#pagesizeProduct").val();
    $.ajax({
        url: "/Product/ChangePageSize",
        data: { pageSizeProduct: pageSizeProduct },
        success: function (response) {
            if (response.success) {
                $("#productListdata").load("productShared");
                //window.location.reload();
            }
        }
    });
}


function changePageInProductTable(productPageNumber) {
    console.log(productPageNumber);
    $.ajax({
        url: "/Product/ChangePage",
        data: { productPageNumber: productPageNumber },
        success: function (response) {
            if (response.success) {
                $("#productListdata").load("productShared");
                //window.location.reload();
            }
        }
    });
}


//////////Category Script////////////
function changePageSize() {
    var catPageSize = $("#pagesizeCategory").val();
    console.log(catPageSize);
    $.ajax({
        url: "/Category/ChangePageSize",
        data: { catPageSize: catPageSize },
        success: function (response) {
            if (response.success) {
                $("#categoryListdata").load("/Category/listshared"); // Ensure this URL is correct
            }
        }
    });
}



function changePageInTable(pageNumberCategory) {
    console.log(pageNumberCategory);
    $.ajax({
        url: "/Category/ChangePage",
        data: { pageNumberCategory: pageNumberCategory },
        success: function (response) {
            if (response.success) {
                $("#categoryListdata").load("listshared");
                //window.location.reload();
            }
        }
    });
}


//function changeProductpagesize() {
//    console.log("hi")
//    var pageSize = $("#pagesize").val();
//    $.ajax({
//        url: "/Product/ChangePageSize",
//        data: { pageSize: pageSize },
//        success: function (response) {
//            if (response.success) {
//                $("#productListdata").load("productShared");
//                //window.location.reload();
//            }
//        }
//    });
//}

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

function searchCode() {

    var searchCode = $("#searchCategoryCodeTab").val();
    var searchName = $("#searchCategoryTab").val();
    var description = $("#searchCategoryDescription").val();
    //console.log(searchCode);

    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { searchCode: searchCode, searchName: searchName, description: description },
        type: 'GET',
        success: function (result) {
            //console.log(result);
            $("#categoryListdata").html(result);
        }
    });
}


function searchCategoryDescription() {

    var description = $("#searchCategoryDescription").val();
    var searchName = $("#searchCategoryTab").val();
    var searchCode = $("#searchCategoryCodeTab").val();

    //console.log(description);

    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { description: description, searchName: searchName, searchCode: searchCode },
        type: 'GET',
        success: function (result) {
            //console.log(result);
            $("#categoryListdata").html(result);
        }
    });
}

function CategoryFilter(sortType) {
    var searchCode = $("#searchCategoryCodeTab").val();
    var searchName = $("#searchCategoryTab").val();
    var description = $("#searchCategoryDescription").val();
    $.ajax({
        url: 'listshared',
        type: 'POST',
        data: { sortType: sortType, searchCode: searchCode, searchName: searchName, description: description },
        success: function (result) {
            //console.log(result);
            $("#categoryListdata").html(result);
        }
    });
}

function ProductFilter(sortTypeProduct) {
    var searchDescription = $("#searchDescription").val();
    var searchProduct = $("#searchProduct").val();
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchCategory = $("#searchCategory").val();
    $.ajax({
        url: 'productShared',
        type: 'POST',
        data: { sortTypeProduct: sortTypeProduct, searchDescription: searchDescription, searchProduct: searchProduct, searchCategoryTag: searchCategoryTag, searchCategory: searchCategory },
        success: function (result) {
            //console.log(result);
            $("#productListdata").html(result);
        }
    });
}

function searchProductCategory() {
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