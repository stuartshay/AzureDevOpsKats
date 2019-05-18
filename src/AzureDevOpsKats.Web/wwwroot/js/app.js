var bytes = [],
    selectedId = '',
    file = null;

$( window ).resize(function() {
    $(".card-img-top").height($(".card-img-top").width() * .7 + "px");
});

function getCats() {
    $("#cat-list").empty();
    $.get("/api/v1/Cats", function( data ) {
        $.each(data, function(index, d) {
            
            var html = `<div class="col-sm-12 col-md-4">
                    <div class="card mt-3 mb-3">
                        <img class="card-img-top" src="${d.photo}" alt="Card image">
                        <div class="card-body">
                        <h4 class="card-title">${d.name}</h4>
                        <p class="card-text">${d.description}</p>
                        <button class="btn btn-success" onclick="editCatDialog(${d.id})">Edit</button>
                        <button class="btn btn-danger" onclick="deleteCat(${d.id})">Delete</button>
                        </div>
                    </div>
                </div>`;

            $("#cat-list").append(html);
            $(".card-img-top").height($(".card-img-top").width() * .7 + "px");
        });
    });
}

getCats();

function addCat() {
    // var formData = new FormData();
    // formData.append('name', $("#add-form #name").val());
    // formData.append('description', $("#add-form #desc").val());
    // formData.append('file', file);
    // $.post('/api/Cats', JSON.stringify({
    //     name: $("#add-form #name").val(),
    //     description: $("#add-form #desc").val(),
    //     file: file
    // }), function(data, error) {
    //     console.log(error)
    //     getCats();
    //     $("#addModal").hide();
    // });
    $.ajax({
        url: `/api/v1/Cats`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            name: $("#add-form #name").val(),
            description: $("#add-form #desc").val(),
            file: file
        }),
        success: function(result) {
            $("#addModal").hide();
            getCats();
        },
        error: function(error) {
            console.log(error)
        }
    });
}

function editCat() {
    $.ajax({
        url: `/api/v1/Cats/${selectedId}`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify({
            name: $("#edit-form #name").val(),
            description: $("#edit-form #desc").val(),
        }),
        success: function(result) {
            $("#editModal").hide();
            getCats();
        },
        error: function(error) {
            console.log(error)
        }
    });
}

function deleteCat(id) {
    $.ajax({
        url: `/api/v1/Cats/${id}`,
        type: 'DELETE',
        success: function(result) {
            getCats();
        }
    });
}

function addCatDialog() {
    $("#addModal").show();
}

function editCatDialog(id) {
    selectedId = id;
    $("#editModal").show();
    $.get(`/api/v1/Cats/${id}`, function(data) {
        $("#edit-form #name").val(data.name);
        $("#edit-form #desc").val(data.description);
    });
}

function closeAddModal() {
    $("#addModal").hide();
}

function closeEditModal() {
    $("#editModal").hide();
}

function changeFile(e) {
    file = e.target.files[0];
    var reader = new FileReader();
    reader.onload = function() {
        var arrayBuffer = this.result;
        bytes = new Uint8Array(arrayBuffer);
        // binaryString = String.fromCharCode.apply(null, array);
    }
    reader.readAsArrayBuffer(e.target.files[0]);
}
