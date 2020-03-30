(function (taxiDeleteDialog) {

    var methods = {
        "openModal": openModal,
        "deleteItem": deleteItem
    };

    var item_to_delete;

    /**
         * Open a modal by class name or Id.
         *
         * @return string id item.
         */
    function openModal(modalName, classOrId, sourceEvent, deletePath, eventClassOrId) {
        var textEvent;
        if (classOrId) {
            textEvent = "." + modalName;
        } else {
            textEvent = "#" + modalName;
        }
        $(textEvent).click((e) => {
            item_to_delete = e.currentTarget.dataset.id;
            deleteItem(sourceEvent, deletePath, eventClassOrId);
        });
    }

    /**
     * Path to delete an item.
     *
     * @return void.
     */
    function deleteItem(sourceEvent, deletePath, eventClassOrId) {
        var textEvent;
        if (eventClassOrId) {
            textEvent = "." + sourceEvent;
        } else {
            textEvent = "#" + sourceEvent;
        }
        $(textEvent).click(function () {
            window.location.href = deletePath + item_to_delete;
        });
    }

    taxiDeleteDialog.sc_deleteDialog = methods;

})(window);
