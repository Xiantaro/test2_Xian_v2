//sec1
$("#Qbtn_search1").on("click", () => {
    if ($("#Qbox_search").val() !== "") { $("#box_search").val($("#Qbox_search").val()) }

    $("#box_status").val("1")
    $("#btn_submit1").trigger("click")
})

$("#Qbtn_search3").on("click", () => {
    let x = $("#Qbox_search_type").val();
    let y = parseInt($("#Qbox_search_year1").val(), 10);
    let z = parseInt($("#Qbox_search_year2").val(), 10);
    let a = $("#Qsel_type1").val();
    let b = $("#Qsel_lang").val();
    let c = $("#Qsel_type2").val();

    $("#Qbox_search_year1").val(($("#Qbox_search_year1").val() !== "" && !isNaN(y)) ? (y >= 1950 ? (y <= 2025 ? (y <= z ? y : 1950) : 1950) : 1950) : 1950)
    $("#Qbox_search_year2").val(($("#Qbox_search_year2").val() !== "" && !isNaN(z)) ? (z >= 1950 ? (z <= 2025 ? (z >= y ? z : 2025) : 2025) : 2025) : 2025)

    $("#box_search_type").val(x)
    $("#box_search_year1").val($("#Qbox_search_year1").val())
    $("#box_search_year2").val($("#Qbox_search_year2").val())
    $("#sel_type1").val(a)
    $("#sel_lang").val(b)
    $("#sel_type2").val(c)

    $("#box_status").val("2")
    $("#btn_submit2").trigger("click")
})

$("#Qbtn_search2").on("click", () => {
    $("#box_query2").removeClass("d-none")
    $("#box_query1").addClass("d-none")

    if ($("#Qbox_search").val() !== "") { $("#Qbox_search_type").val($("#Qbox_search").val()) }
})

$("#Qbtn_search4").on("click", () => {
    $("#box_query1").removeClass("d-none")
    $("#box_query2").addClass("d-none")

    if ($("#Qbox_search_type").val() !== "") { $("#Qbox_search").val($("#Qbox_search_type").val()) }
})

$(".Qbtn_hot").on("click", (e) => {
    let x = $(e.currentTarget).text();
    let a = $("#Qsel_type1").val();
    let b = $("#Qsel_lang").val();
    let c = $("#Qsel_type2").val();

    if ($("#box_query2").hasClass("d-none")) {
        $("#box_search").val(x)
        $("#btn_search1").trigger("click")
    }
    else if ($("#box_query1").hasClass("d-none")) {
        $("#box_search_type").val(x)
        $("#sel_type1").val(a)
        $("#sel_lang").val(b)
        $("#sel_type2").val(c)
        $("#btn_search3").trigger("click")
    }
})

$("figure").on("click", function () {
    let x = $(this).find("figcaption").text()

    $("#box_search").val(x)

    $("#box_status").val("1")
    $("#btn_submit1").trigger("click")
})

//interaction
chkStatus1 = () => {
    let x = $("#Qbox_status").val();

    if (x === "2") {
        $("#box_query2").removeClass("d-none")
        $("#box_query1").addClass("d-none")
    }

    if ($("#Qbox_search").val() !== "") {
        $("#box_result").removeClass("d-none")
        $("#sec2").removeClass("d-none")
    } else {
        $("#box_result").addClass("d-none")
        $("#sec2").addClass("d-none")
    }
}

chkStatus2 = () => {
    if ($("#Qbox_search").val() !== "" || $("#Qbox_search_type").val() !== "") {
        $("#box_result").removeClass("d-none")
        $("#sec2").removeClass("d-none")
    } else {
        $("#box_result").addClass("d-none")
        $("#sec2").addClass("d-none")
    }
}

chkSel = () => {
    const x = $("#Qsel_type1")
    const y = $("#Qsel_lang")
    const z = $("#Qsel_type2")

    const a = x.data("selected-value")
    const b = y.data("selected-value")
    const c = z.data("selected-value")

    if (a !== undefined && a !== null && a !== "") { x.val(a) }
    if (b !== undefined && b !== null && b !== "") { y.val(b) }
    if (c !== undefined && c !== null && c !== "") { z.val(c) }
}

$(() => { chkStatus1(); chkStatus2(); chkSel() })