$(document).ready(function () {
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
        let row = $(this).closest('tr');
        let id = row.find('td[hidden]').attr("id");
        let imageSrc = row.find('td img').attr("src");
        let name = row.find('td[name]').attr("name");
        let description = row.find('td[description]').attr("description");
        let verified = row.find('td input').prop("checked");

        $("#editId").val(id);
        $("#editName").val(name);
        $("#editDescription").val(description);
        $("#editImageName").attr("value", imageSrc.replace("/Contents/", ""));
        $("#editVerified").prop("checked", verified);
        $("#editImagePreview").attr("src", imageSrc);

        let editModal = new bootstrap.Modal(document.getElementById('EditModal'));
        editModal.show();
    });

    // Delete button
    $(".delete").on("click", function () {
        let id = $(this).closest('tr').find('td[hidden]').attr("id");
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
        let row = $(this).closest('tr');
        let id = row.find('td[hidden]').attr("id");
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