$("#addItem").click(function () {
    $.ajax({
        url: this.href + "?idAnterior=" + $("#editorRows .deleteRow").length,
        cache: false,
        success: function (html) { $("#editorRows").append(html); }
    });
    return false;
});

$(document).on('click', ".deleteRow", function () {
    console.log(this);
    $(this).parent().remove();
    return false;
});

function b64EncodeUnicode(str) {
    // first we use encodeURIComponent to get percent-encoded UTF-8,
    // then we convert the percent encodings into raw bytes which
    // can be fed into btoa.
    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
        function toSolidBytes(match, p1) {
            return String.fromCharCode('0x' + p1);
        }));
}

$("#submitButton").click(function () {

    $("#submitButton").html('<span class="spinner-border spinner-border-sm" role="status"></span> Enviando...');
    rawCodigo = ace.edit("editor").getValue();
    codigo = btoa(unescape(encodeURIComponent(rawCodigo)));
    id = $("#contest").val();
    problema = $("#problema").val();
    url1 = "/Arena/Problem/?Id=" + id + "&problem=" + problema + "&code=" + codigo;
    $.ajax({
        type: "POST",
        url: url1,
        success: function (msg) {
            location.reload();
        }
    });
    return false;
})

$('*[data-href]').on('click', function () {
        window.location = $(this).data("href");
    });

/**
 * 
 * model ArenaOIA.Models.Problem
    <div class="editorRow">
        Item: @Html.TextBoxFor(x => x.Nombre)
        Value: @Html.TextBoxFor(x => x.Puntaje)
        <a href="#" class="deleteRow">Eliminar</a>
    </div>

 * */