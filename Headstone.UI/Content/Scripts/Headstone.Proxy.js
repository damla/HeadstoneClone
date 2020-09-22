var Headstone = Headstone || {};
Headstone.Service = {};

Headstone.Service.Basket = {
    AddToBasket: function (productId, name, successCallback, errorCallback) {
        var controller = "basket";
        var action = "addtobasket";

        var product = {
            ProductId = productId,
            Name = name
        };

        $.ajax({
            type: "POST",
            url: "/" + controller + "/" + action,
            data: JSON.stringify(product),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: successCallback,
            error: errorCallback
        }
};