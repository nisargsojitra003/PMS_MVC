// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function handleDeleteProductClick() {
//    var id = undefined;

    $(document).on('click', '.deleteProduct', function () {
        $('#DeleteProductModel').load("/dashboard/LoadModal");
        $("#delModal").modal("show");
        //$("#modalValue").val($(this).closest('.tr').data('rid'));
        id = $(this).closest('.tr').data('rid');
        $("#MainLabel").html("Delete Product?");
        $("#confirmLabel").html("<strong>Are you sure to want to delete this Product?</strong>");
        $("#DeleteButton").addClass("DeleteProductForm");
    });

    $(document).on("click", ".DeleteProductForm", function () {
        //var id = $("#modalValue").html();
        //console.log(id);
        $.ajax({
            url: '/Product/Delete',
            data: { id: id },
            beforeSend: function ()
            {
                console.log("display");
                DisplayLoader();
            },
            success: function (response)
            {
                DisplayLoader();
                if (response.success)
                {
                    window.alert("arrive");
                    toastr.success('Product has been deleted successfully!');
                    HideLoader();
                    $('#delModal').modal("toggle");
                    $('#productListdata').load("/product/ProductShared");
                }
                else
                {
                    toastr.error("Something went Wrong!");
                    HideLoader();
                    $('#delModal').modal("toggle");
                    $('#productListdata').load("/product/ProductShared");
                }
            }
        });
    });

    $('#delModal').on('hidden.bs.modal', function () {
        $('#DeleteProductModel').empty();
        id = undefined;
        $("#MainLabel").empty();
        $("#confirmLabel").empty();
        $("#DeleteButton").removeClass("DeleteProductForm");
    });
//}

//function handleDeleteCategoryClick() {
//    var id = undefined;
    $(document).on('click', '.deleteCategory', function () {
        $('#DeleteCategoryModal').load("/dashboard/LoadModal");
        //$("#modalValue").html($(this).closest('.tr').data('rid'));
        id = $(this).closest('.tr').data('rid');
        $("#modalValue").val(id);
        console.log($("#modalValue").val());
        $("#MainLabel").html("Delete Category?");
        $("#confirmLabel").html("<strong>Are you sure you want to delete this Category?</strong>");
        $("#DeleteButton").addClass("DeleteCategoryForm");
        $("#delModal").modal("show"); // Corrected modal opening
    });


    $(document).on("click", ".DeleteCategoryForm", function () {

        $.ajax({
            url: '/Category/Delete',
            data: { id: id },
            beforeSend: function () {
                console.log("Display Loader");
                DisplayLoader();
            },
            success: function (response) {
                HideLoader();
                if (response.success) {
                    toastr.success('Category has been deleted successfully!');
                    HideLoader();
                    $('#delModal').modal("toggle");
                    $('#categoryListdata').load("/Category/listshared");
                } else {
                    toastr.error("Selected Category have already products so you can't delete this category!");
                    HideLoader();
                    $('#delModal').modal("toggle");
                    $('#categoryListdata').load("/Category/listshared");
                }
            }
        });
    });
//}

function validateForm() {
    var fileInput = document.getElementById("myFile");
    var file = fileInput.files[0];
    if (!file) {
        alert("Please select a file.");
        return false;
    }

    var allowedTypes = ["image/jpeg", "image/png", "image/jpg"];
    if (!allowedTypes.includes(file.type)) {
        alert("Only JPEG, PNG, and JPG files are allowed.");
        return false;
    }

    var maxSize = 5 * 1024 * 1024;
    if (file.size > maxSize) {
        alert("File size exceeds the maximum limit of 5MB.");
        return false;
    }
    return true;
}

function searchCategory() {
    var searchName = $("#searchCategoryTab").val();
    var searchCode = $("#searchCategoryCodeTab").val();
    var description = $("#searchCategoryDescription").val();
    console.log(searchName);
    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { searchName: searchName, searchCode: searchCode, description: description },
        type: 'GET',
        beforeSend: function () {
            console.log("display");
            DisplayLoader();
            debugger;
        },
        success: function (result) {
            console.log("complete");
            $("#categoryListdata").html(result);
        },
        complete: function () {
            console.log("hide");
            HideLoader();
        },
        error: function (error) {
            console.log('Error:', error);
            HideLoader();
        }
    });
}

function searchProcudtName() {

    var searchProduct = $("#searchProduct").val();
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchDescription = $("#searchDescription").val();
    var searchCategory = $("#searchCategory").val();
   
    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchProduct: searchProduct, searchCategoryTag: searchCategoryTag, searchDescription: searchDescription, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
            $("#productListdata").html(result);
        }
    });
}

function searchCategoryTag() {
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchProduct = $("#searchProduct").val();
    var searchDescription = $("#searchDescription").val();
    var searchCategory = $("#searchCategory").val();

    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchCategoryTag: searchCategoryTag, searchProduct: searchProduct, searchDescription: searchDescription, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
            $("#productListdata").html(result);
        }
    });
}

function searchDescription() {
    var searchDescription = $("#searchDescription").val();
    var searchProduct = $("#searchProduct").val();
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchCategory = $("#searchCategory").val();

    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchDescription: searchDescription, searchProduct: searchProduct, searchCategoryTag: searchCategoryTag, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
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
            }
        }
    });
}


function changePageInProductTable(productPageNumber) {
    $.ajax({
        url: "/Product/ChangePage",
        data: { productPageNumber: productPageNumber },
        success: function (response) {
            if (response.success) {
                $("#productListdata").load("productShared");
            }
        }
    });
}


//////////Category Script////////////
function changePageSize() {
    var catPageSize = $("#pagesizeCategory").val();
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
    $.ajax({
        url: "/Category/ChangePage",
        data: { pageNumberCategory: pageNumberCategory },
        success: function (response) {
            if (response.success) {
                $("#categoryListdata").load("listshared");
            }
        }
    });
}


/////////////////////////Activity Page////////////////////////////
function changePageSizeActivity() {
    var activityPageSize = $("#pagesizeActivity").val();
    $.ajax({
        url: "/dashboard/ChangeActivityPageSize",
        data: { activityPageSize: activityPageSize },
        success: function (response) {
            if (response.success) {
                $("#activityListdata").load("/dashboard/userActivityShared"); // Ensure this URL is correct
            }
        }
    });
}

function changePageInActivityTable(activityPageNumber) {
    $.ajax({
        url: "/dashboard/ChangeActivityPage",
        data: { activityPageNumber: activityPageNumber },
        success: function (response) {
            if (response.success) {
                $("#activityListdata").load("/dashboard/userActivityShared");
            }
        }
    });
}

function searchActivity() {

    var searchActivity = $("#searchActivityTab").val();

    let url = '/dashboard/userActivityShared';
    $.ajax({
        url: url,
        data: { searchActivity: searchActivity },
        type: 'GET',
        success: function (result) {
            $("#activityListdata").html(result);
        }
    });
}


function ActivityFilter(sortTypeActivity) {
    var searchActivity = $("#searchActivityTab").val();

    $.ajax({
        url: '/dashboard/userActivityShared',
        type: 'POST',
        data: { searchActivity: searchActivity, sortTypeActivity: sortTypeActivity },
        success: function (result) {
            $("#activityListdata").html(result);
        }
    });
}


function searchCategory() {

    var searchName = $("#searchCategoryTab").val();
    var searchCode = $("#searchCategoryCodeTab").val();
    var description = $("#searchCategoryDescription").val();

    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { searchName: searchName, searchCode: searchCode, description: description },
        type: 'GET',
        success: function (result) {
            $("#categoryListdata").html(result);
        }
    });
}

function searchCode() {

    var searchCode = $("#searchCategoryCodeTab").val();
    var searchName = $("#searchCategoryTab").val();
    var description = $("#searchCategoryDescription").val();

    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { searchCode: searchCode, searchName: searchName, description: description },
        type: 'GET',
        success: function (result) {
            $("#categoryListdata").html(result);
        }
    });
}


function searchCategoryDescription() {

    var description = $("#searchCategoryDescription").val();
    var searchName = $("#searchCategoryTab").val();
    var searchCode = $("#searchCategoryCodeTab").val();


    let url = '/Category/listshared';
    $.ajax({
        url: url,
        data: { description: description, searchName: searchName, searchCode: searchCode },
        type: 'GET',
        success: function (result) {
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
            $("#productListdata").html(result);
        }
    });
}

function searchProductCategory() {
    var searchProduct = $("#searchProduct").val();
    var searchCategoryTag = $("#searchCategoryTag").val();
    var searchDescription = $("#searchDescription").val();
    var searchCategory = $("#searchCategory").val();

    let url = '/Product/productShared';
    $.ajax({
        url: url,
        data: { searchProduct: searchProduct, searchCategoryTag: searchCategoryTag, searchDescription: searchDescription, searchCategory: searchCategory },
        type: 'GET',
        success: function (result) {
            $("#productListdata").html(result);
        }
    });
}


function DeleteCategoryByValue() {
    var id = $('#CategoryIdVal').val();
    $.ajax({
        url: '/Category/Delete',
        data: { id: id },
        beforeSend: function () {
            console.log("display");
            DisplayLoader();
            debugger;
        },
        success: function (response) {
            if (response.success) {
                toastr.success('Category has been deleted successfully!');
                $('#delCategoryModal').modal('toggle');
                $('#categoryListdata').load("/category/listshared");
            }
            else {
                toastr.error("Selected Category have already products so you can't delete this category!");
                $('#delCategoryModal').modal('toggle');
                $('#categoryListdata').load("/category/listshared");
            }
        }
    });
}

function DeleteProductByValue() {
    var id = $('#ProductIdVal').val();
    $.ajax({
        url: '/Product/Delete',
        data: { id: id },
        beforeSend: function () {
            console.log("display");
            DisplayLoader();
            debugger;
        },
        success: function (response) {
            DisplayLoader();
            if (response.success) {
                toastr.success('Product has been deleted successfully!');
                HideLoader();
                $('#delProductModal').modal('toggle');
                $('#productListdata').load("/product/ProductShared");
            }
            else {
                toastr.error("Something went Wrong!");
                HideLoader();
                $('#delCategoryModal').modal('toggle');
                $('#productListdata').load("/product/ProductShared");   
            }
        }
    });
}

function DisplayLoader() {
    $("#overlay").show();
}

function HideLoader() {
    setTimeout(function () {
        $("#overlay").hide();
    }, 2000);
}

$(window).on("beforeunload", function () {
    DisplayLoader();
});

$(document).on("submit", "form", function () {
    DisplayLoader();
});

$(document).ready(function () {
    HideLoader();
});



//$(document).on("click", ".DeleteProductForm", function () {
//    //var id = $('#modalValue').val();
//    var id = document.getElementById("modalValue").value;
//    console.log(id);
//    $.ajax({
//        url: '/Product/Delete',
//        data: { id: id },
//        beforeSend: function () {
//            console.log("display");
//            DisplayLoader();
//        },
//        success: function (response) {
//            DisplayLoader();
//            if (response.success) {
//                toastr.success('Product has been deleted successfully!');
//                HideLoader();
//                $('#delProductModal').modal('toggle');
//                $('#productListdata').load("/product/ProductShared");
//            }
//            else {
//                toastr.error("Something went Wrong!");
//                HideLoader();
//                $('#delCategoryModal').modal('toggle');
//                $('#productListdata').load("/product/ProductShared");
//            }
//        }
//    });
//});