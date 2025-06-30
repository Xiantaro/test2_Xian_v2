$("#btn_search1").on("click", () => {
    if ($("#box_search").val() === "") { $("#box_result").addClass("d-none") }

    $("#box_status").val("1")
    $("#btn_submit1").trigger("click")
    $(".btn_guideQ").trigger("click")
})

$("#btn_search3").on("click", () => {
    let x = parseInt($("#box_search_year1").val(), 10);
    let y = parseInt($("#box_search_year2").val(), 10);

    $("#box_search_year1").val(($("#box_search_year1").val() !== "" && !isNaN(x)) ? (x >= 1950 ? (x <= 2025 ? (x <= y ? x : 1950) : 1950) : 1950) : 1950)
    $("#box_search_year2").val(($("#box_search_year2").val() !== "" && !isNaN(y)) ? (y >= 1950 ? (y <= 2025 ? (y >= x ? y : 2025) : 2025) : 2025) : 2025)

    if ($("#box_search_type").val() === "") { $("#box_result").addClass("d-none") }

    $("#box_status").val("2")
    $("#btn_submit2").trigger("click")
    $(".btn_guideQ").trigger("click")
})

$("#btn_search2").on("click", () => {
    $("#box_modal2").removeClass("d-none")
    $("#box_modal1").addClass("d-none")
})

$("#btn_search4").on("click", () => {
    $("#box_modal1").removeClass("d-none")
    $("#box_modal2").addClass("d-none")
})

$(".btn_hot").on("click", (e) => {
    let x = $(e.currentTarget).text();

    if ($("#box_modal2").hasClass("d-none")) {
        $("#box_search").val(x)
        $("#btn_search1").trigger("click")
    }
    else if ($("#box_modal1").hasClass("d-none")) {
        $("#box_search_type").val(x)
        $("#btn_search3").trigger("click")
    }
})