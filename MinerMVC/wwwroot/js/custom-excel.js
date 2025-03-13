$(document).ready(function () {
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
        let verified = row.find('td input').is(":checked");

        $("#editId").val(id);
        $("#editName").val(name);
        $("#editDescription").val(description);
        $("#editImageName").attr("value", imageSrc.replace("/Contents/", ""));
        $("#editVerified").attr("checked", verified).val(verified);
        $("#editImagePreview").attr("src", imageSrc);

        let editModal = new bootstrap.Modal(document.getElementById('EditModal'));
        editModal.show();
    });

    // Delete button
    $(".delete").on("click", function () {
        let id = $(this).closest('tr').find('td[hidden]').attr("id");
        $("#customExcelId").attr("value", id);
        let deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
        deleteModal.show();
    });

    // Image preview for insert
    $("#insertImage").change(function () {
        let file = this.files[0];
        let reader = new FileReader();
        reader.onload = function (e) {
            $("#insertImagePreview").attr("src", e.target.result);
        };
        reader.readAsDataURL(file);
    });

    // Image preview for edit
    $("#editImage").change(function () {
        let file = this.files[0];
        let reader = new FileReader();
        reader.onload = function (e) {
            $("#editImagePreview").attr("src", e.target.result);
        };
        reader.readAsDataURL(file);
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
}); 