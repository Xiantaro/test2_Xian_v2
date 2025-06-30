$(".btn_book").on("click", function () {
    let boxBook = $(this).closest(".box_book");
    let bookTable = boxBook.find("table");
    let bookTitle = bookTable.find("h4").text();
    //let bookAuthor = bookTable.find("th:contains('作者')").next("td").text();
    //let bookTranslator = bookTable.find("th:contains('譯者')").next("td").text();
    //let bookPublisher = bookTable.find("th:contains('出版社')").next("td").text();
    //let bookLanguage = bookTable.find("th:contains('語言')").next("td").text();
    //let bookIsbn = bookTable.find("th:contains('ISBN')").next("td").text();

    $("#box_search").val(bookTitle)
    $("#btn_search1").trigger("click")
})

//interaction
$(".page-item").on("click", function () {
    const man = $("#box_man i");
    let x = $(this).data("bs-slide-to");
    let y = x % 5;

    if (x !== undefined) {
        man.css("color", "var(--c-gray3-100)");

        if (y >= 0 && y < man.length) {
            man.eq(y).css("color", "black");
        }
    }

    window.scrollTo({ top: 0, behavior: "smooth" });
})

$("#btn_prev").on("click", () => {
    $("#box_man i").css("color", "var(--c-gray3-100)");
    $("#man1").css("color", "black");

    window.scrollTo({ top: 0, behavior: "smooth" });
})

$("#btn_next").on("click", () => {
    $("#box_man i").css("color", "var(--c-gray3-100)");
    $("#man5").css("color", "black");

    window.scrollTo({ top: 0, behavior: "smooth" });
})