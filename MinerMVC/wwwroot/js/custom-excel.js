$(document).ready(function () {
    // Initialize SortableJS for the card grid
    var cardContainer = document.getElementById('card-container');
    if (cardContainer) {
        new Sortable(cardContainer, {
            animation: 150,
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            handle: '.excel-card', // The entire card acts as a drag handle
            onEnd: function(evt) {
                // You can add AJAX call here to save the new order to backend
                console.log('Card reordered: ' + evt.oldIndex + ' -> ' + evt.newIndex);
                
                // Get all card IDs in their new order
                var cards = Array.from(cardContainer.children);
                var newOrder = cards.map(function(card) {
                    return card.getAttribute('data-id');
                });
                
                // Log the new order (can be sent to server)
                console.log('New order:', newOrder);
            }
        });
    }

    // Refresh button
    $("#refreshButton").on("click", function () {
        window.location.reload();
    });

    // Image preview
    $(".image-thumbnail").on("click", function () {
        let imageSrc = $(this).attr("src");
        $("#modalImage").attr("src", imageSrc);
        let imageModal = new bootstrap.Modal(document.getElementById('imageModal'));
        imageModal.show();
    });

    // Edit button
    $(".edit").on("click", function () {
        let id = $(this).data("id");
        let name = $(this).data("name");
        let description = $(this).data("description");
        let image = $(this).data("image");
        let verified = $(this).data("verified").toLowerCase() === "true";
        
        $("#editId").val(id);
        $("#editName").val(name);
        $("#editDescription").val(description);
        $("#editImageName").attr("value", image);
        $("#editVerified").prop("checked", verified);
        $("#editImagePreview").attr("src", "/Contents/" + image);

        let editModal = new bootstrap.Modal(document.getElementById('EditModal'));
        editModal.show();
    });

    // Delete button
    $(".delete").on("click", function () {
        let id = $(this).data("id");
        $("#customExcelId").val(id);
        let deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
        deleteModal.show();
    });

    // Delete form submit
    $("#deleteForm").on("submit", function (e) {
        e.preventDefault();
        let id = $("#customExcelId").val();
        
        $.ajax({
            url: '/CustomExcel/Delete',
            type: 'GET',
            data: { id: id },
            success: function () {
                window.location.reload();
            }
        });

        let deleteModal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
        deleteModal.hide();
    });

    // Image preview for insert
    $("#insertImage").change(function () {
        let file = this.files[0];
        let preview = $("#insertImagePreview");
        if (file) {
            let reader = new FileReader();
            reader.onload = function (e) {
                preview.attr("src", e.target.result);
                preview.show();
            };
            reader.readAsDataURL(file);
        } else {
            preview.hide();
        }
    });

    // Image preview for edit
    $("#editImage").change(function () {
        let file = this.files[0];
        let preview = $("#editImagePreview");
        if (file) {
            let reader = new FileReader();
            reader.onload = function (e) {
                preview.attr("src", e.target.result);
                preview.show();
            };
            reader.readAsDataURL(file);
        }
    });

    // Checkbox status update
    $(".excel-checkbox").change(function () {
        let card = $(this).closest('.excel-card');
        let id = card.data("id");
        let isChecked = $(this).is(":checked");
        
        $.ajax({
            url: '/CustomExcel/UpdateVerifiedStatus',
            type: 'PUT',
            data: { id: id, status: isChecked }
        });
    });

    // Edit form submit
    $("form[asp-action='Edit']").on("submit", function (e) {
        e.preventDefault();
        let formData = new FormData(this);
        
        // 特別處理 checkbox 的值
        formData.set("Verified", $("#editVerified").prop("checked"));
        
        $.ajax({
            url: '/CustomExcel/Edit',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function () {
                window.location.reload();
            }
        });

        let editModal = bootstrap.Modal.getInstance(document.getElementById('EditModal'));
        editModal.hide();
    });
}); 