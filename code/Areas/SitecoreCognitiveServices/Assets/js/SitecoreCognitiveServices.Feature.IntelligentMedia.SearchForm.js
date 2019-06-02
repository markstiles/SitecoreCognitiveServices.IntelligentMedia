jQuery.noConflict();

//image search
jQuery(document).ready(function () {
    var isInModal = false;

    jQuery(".ui-dialog").mouseenter(function () { isInModal = true; console.log(isInModal) });
    jQuery(".ui-dialog").mouseleave(function () { isInModal = false; console.log(isInModal) });
    function SetupChosen() {
        var config = {
            '.chosen-select': { width: "100%" }
        }
        var results = [];
        for (var selector in config) {
            var elements = jQuery(selector);
            for (var i = 0; i < elements.length; i++) {
                results.push(new Chosen(elements[i], config[selector]));
            }
        }
    }

    SetupChosen();

    jQuery(".slider-range").each(function () {
        var isInModal = false;
        var isSliding = false;
        var slider = jQuery(this);

        jQuery(this).slider({
            range: true,
            min: 0,
            max: 100,
            values: [0, 100],
            slide: function (event, ui) {
                var parent = jQuery(this).parent('.emotion-item');
                var filterValue = jQuery(parent).find(".filter-value");
                var rangeVal = ui.values[0] + " - " + ui.values[1];
                rangeVal += (filterValue.data("field") == "age") ? "" : "%";
                filterValue.html(rangeVal);
                filterValue.data('min', ui.values[0]);
                filterValue.data('max', ui.values[1]);

                // Trigger stop function if user slides handle off modal
                isSliding = true;
                jQuery(".result-filters").mouseenter(function () { isInModal = true; });
                jQuery(".result-filters").mouseleave(function () {
                    isInModal = false;
                    if (isSliding) {
                        slider.slider("instance").options.stop();
                        isSliding = false;
                    }
                });
            },
            stop: function (event, ui) {
                if (isSliding) {  // Check prevents stop from running twice
                    isSliding = false;
                    jQuery(".result-filters").unbind("mouseenter");
                    jQuery(".result-filters").unbind("mouseleave");
                    RefreshQuery();
                }
            }
        });

        var parent = jQuery(this).parent('.emotion-item');
        var filterValue = jQuery(parent).find(".filter-value");
        var htmlVal = "0 - 100";
        htmlVal += (filterValue.data("field") == "age") ? "" : "%";
        filterValue.html(htmlVal);
        filterValue.data('min', 0);
        filterValue.data('max', 100);
    });

    jQuery(".slider-range-size").each(function () {
        jQuery(this).slider({
            range: true,
            min: 0,
            max: 4000,
            values: [0, 4000],
            slide: function (event, ui) {
                var parent = jQuery(this).parent('.emotion-item');
                var filterValue = jQuery(parent).find(".filter-value");
                filterValue.html(ui.values[0] + " - " + ui.values[1]);
                filterValue.data('min', ui.values[0]);
                filterValue.data('max', ui.values[1]);
            }
        });

        var parent = jQuery(this).parent('.emotion-item');
        var filterValue = jQuery(parent).find(".filter-value");
        filterValue.html("0 - 4000");
        filterValue.data('min', 0);
        filterValue.data('max', 4000);
    });

    //closes modal and send selected image back to RTE
    var imageSearchForm = ".image-search-form";
    var rteSearchForm = ".rte-search-form";
    jQuery(imageSearchForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var img = jQuery(".result-items .selected");
            if (img.length)
                CloseModalAndReturnValue();
            else
                alert(jQuery(".select-an-image").text());
        });

    //closes modal on cancel press
    jQuery(imageSearchForm + " .form-cancel")
        .click(function (event) {
            event.preventDefault();

            CloseModal();
        });

    function RefreshQuery() {
        pageNum = 1;
        RunQuery();
    }

    //reset form
    jQuery(imageSearchForm + " .form-reset")
        .click(function (event) {
            event.preventDefault();

            jQuery(imageSearchForm + " #gender").val(0);
            jQuery(imageSearchForm + " #glasses").val(-1);

            var sliders = jQuery(".slider-range, .slider-range-size");
            for (var i = 0; i < sliders.length; i++) {
                var slider = sliders[i];
                var filterValue = jQuery(slider).parent().find(".filter-value");
                var max = jQuery(slider).slider("option", "max");
                var min = jQuery(slider).slider("option", "min");
                jQuery(filterValue).data('min', min);
                jQuery(filterValue).data('max', max);
                jQuery(slider).slider("values", [min, max]);
                var rangeVal = min + " - " + max;
                rangeVal += (jQuery(filterValue).data("field") == "age") ? "" : "%";
                jQuery(filterValue).html(rangeVal);
            }

            jQuery(".color-count").html("");
            jQuery(".color-item.selected").removeClass("selected");
            jQuery(".color-group-title.group-selected").removeClass("group-selected");

            jQuery(".chosen-container").remove();
            jQuery('.chosen-select option:selected').removeAttr('selected');

            SetupChosen();
            RunQuery();
        });

    var imageSearchSelect = ".filter-section select";
    jQuery(imageSearchSelect).change(function () {
        RefreshQuery();
    });


    var colorTitle = ".color-title";

    renderCategorizedColors(sortColors());

    jQuery(colorTitle).click(function () {
        var colorList = jQuery(this).parent();
        if (colorList.hasClass("open"))
            colorList.removeClass("open");
        else
            colorList.addClass("open");
    });

    /**
     * Classifies and sorts the colors in the color list dropdown.
     * This uses the color-classifier library.  See https://github.com/tsuyoshiwada/color-classifier
     * for documentation
     * @returns {Array} Returns an array of objects.  Each object contains a list of colors, a classifier color name and value
     */
    function sortColors() {
        // Colors will be classified based on how closely they match these defined colors
        const palette = [
            { name: "Red", value: "#DC143C" },
            { name: "Orange", value: "#D85A01" },
            { name: "Yellow", value: "#CCCC00" },
            { name: "Green", value: "#008000" },
            { name: "Blue", value: "#2339EF" },
            { name: "Purple", value: "#8A2BE2" },
            { name: "Pink", value: "#F49FBE" },
            { name: "Brown", value: "#614126" },
            { name: "Black", value: "#000000" },
            { name: "Gray", value: "#696969" },
            { name: "White", value: "#DCDCDC" }
        ];
        let colorClassifier = new ColorClassifier(palette.map(p => p.value));

        // Create a list of hex values generated by the image analysis
        let hexArray = jQuery(".color-list").children(".color-item").map(function () {
            return jQuery(this).attr("value");
        });
        let colorGroups = colorClassifier.classifyFromArray(hexArray.toArray(), "hex");
        colorGroups.map(group => {
            const color = palette.find(p => p.value === group.palette);
            group.paletteName = color.name;
        })
        colorGroups.sort((a, b) => { return (a.paletteName > b.paletteName) ? 1 : ((b.paletteName > a.paletteName) ? -1 : 0); });
        console.log(colorGroups);
        return colorGroups;
    }

    /**
     * Appends an array of color groups to the color list dropdown
     * @param {Array} colorGroups - an array of objects
     */
    function renderCategorizedColors(colorGroups) {
        let colorList = jQuery(".color-list");

        // Look at each group (classified by color)...
        colorGroups.map(group => {

            // ...and look at each color and find the matching html element
            // in the color-list...
            let htmlColors = colorList.children(".color-item").filter(function () {
                return group.colors.includes(jQuery(this).attr("value"))
            });

            // ...then append the classification color and related html colors to a container group
            let groupContainer = document.createElement("div");
            let groupTitle = document.createElement("h1");

            jQuery(groupTitle)
                .addClass("color-group-title")
                .text(group.paletteName);
            jQuery(groupContainer).addClass("color-group");
            jQuery(groupContainer).append(groupTitle);
            jQuery(groupContainer).append(htmlColors);

            colorList.append(groupContainer);
        })
    }

    // Select/Deselect all colors in the group if classification title is clicked
    jQuery(".color-group-title").click(function (e) {
        // If one or more colors in the group is not selected,
        // the group as a whole is considered not selected
        let isSelected = jQuery(this).hasClass("group-selected")
            && jQuery(this).siblings(".color-item").length === jQuery(this).siblings(".selected").length;

        if (isSelected) {
            jQuery(this).removeClass("group-selected");
            jQuery(this).siblings(".color-item").removeClass("selected");
        } else {
            jQuery(this).addClass("group-selected");
            jQuery(this).siblings(".color-item").addClass("selected");
        }

        var colorCount = jQuery(".color-list .group-selected").length;
        var colorCountValue = (colorCount == 0) ? "" : " (" + colorCount + ")";
        jQuery(".color-count").text(colorCountValue);

        RefreshQuery();
    });

    jQuery(".color-items").mouseleave(function () {
        jQuery(this).removeClass("open");
    });

    var imageColor = ".color-item";
    jQuery(imageColor).click(function () {
        var isSelected = jQuery(this).hasClass("selected");

        if (!isSelected)
            jQuery(this).addClass("selected");
        else {
            jQuery(this).removeClass("selected");
            jQuery(this).siblings(".color-group-title").removeClass("group-selected");
        }

        var colorCount = jQuery(".color-list .selected").length;
        var colorCountValue = (colorCount == 0) ? "" : " (" + colorCount + ")";
        jQuery(".color-count").text(colorCountValue);

        RefreshQuery();
    });

    jQuery(".slider-range-size, .chosen-results").mouseup(function (e) {
        if (e.which != 1)
            return;

        RefreshQuery();
    });

    //performs search on 'enter-press' on the form
    jQuery(imageSearchForm + " .search-submit, " + imageSearchForm + " .cognitiveSearchButton")
        .click(function (event) {
            event.preventDefault();

            RefreshQuery();
        });

    //performs image search
    var pageNum = 1;
    var pageSize = 35;
    var pageCount = 1;
    var searchResults;
    function RunQuery() {
        var langValue = jQuery(imageSearchForm + " #language").attr("value");
        var dbValue = jQuery(imageSearchForm + " #database").attr("value");
        var fieldSourceIdValue = jQuery(imageSearchForm + " #fieldSourceId").attr("value");
        var gender = jQuery(imageSearchForm + " #gender").val();
        var glasses = jQuery(imageSearchForm + " #glasses").val();

        jQuery(rteSearchForm + " .progress-indicator").show();
        jQuery(".result-items").hide();

        var colorValue = [];
        jQuery(imageSearchForm + " .color-list .selected").each(function (index) {
            colorValue.push(jQuery(this).attr("value"));
        });
        var tags = GetTagParameters();
        var ranges = GetRangeParameters();

        jQuery.post(
            jQuery(imageSearchForm).attr("action"),
            {
                formParameters: [],
                tagParameters: tags,
                rangeParameters: ranges,
                gender: gender,
                glasses: glasses,
                language: langValue,
                colors: colorValue,
                db: dbValue,
                fieldSourceId: fieldSourceIdValue,
                page: pageNum,
                pageLength: pageSize
            }
        ).done(function (r) {
            searchResults = r;

            jQuery(".pagenum").text(pageNum);
            pageCount = Math.ceil(r.ResultCount / pageSize);
            jQuery(".pagecount").text(pageCount);

            jQuery(".result-count").text(r.ResultCount);
            jQuery(".result-items").text("");
            jQuery(".result-items").show();

            for (var i = 0; i < r.Results.length; i++) {
                var d = r.Results[i];
                if (d.Url != undefined) {
                    jQuery(".result-items").append("<div class='result-img-wrap'><img src=\"" + d.Url + "\" alt=\"" + d.Alt + "\" title=\"" + d.Title + "\" id=\"" + d.Id + "\" /></div>");
                }
            }

            jQuery(".result-img-wrap")
                .on("click", function () {
                    jQuery(".result-items .selected").removeClass("selected");
                    jQuery(this).addClass("selected");
                });

            jQuery(".search-choice-close")
                .on('click', function () {
                    setTimeout(RefreshQuery, 100);
                });
        }).always(function () {
            jQuery(rteSearchForm + " .progress-indicator").hide();
        });
    }

    //changes the current page
    var prevBtn = ".result-nav-prev";
    var nextBtn = ".result-nav-next";
    jQuery(prevBtn).click(function (e) {
        if (pageNum < 2)
            return;

        pageNum--;
        RunQuery();
    });

    jQuery(nextBtn).click(function (e) {
        if ((pageNum + 1) > pageCount)
            return;

        pageNum++;
        RunQuery();
    });

    function GetRangeParameters() {
        var params = [];

        var filterElements = jQuery('.filter-value');
        if (filterElements.length > 0) {

            filterElements.each(function () {
                var values = [];
                values.push(jQuery(this).data('min'));
                values.push(jQuery(this).data('max'));

                params.push({
                    key: jQuery(this).data('field'),
                    value: values
                });
            });
        }

        return params;
    }

    function GetTagParameters() {
        var params = [];
        params.push({
            key: "tags",
            value: [""]
        });

        //tags
        var tagElements = jQuery('.search-choice span');
        if (tagElements.length > 0) {

            var values = [];

            tagElements.each(function () {
                var text = jQuery(this).text();
                text = text.substring(0, text.indexOf(' ('));
                text = text.toLowerCase();
                values.push(text);
            });

            params.push({
                key: "tags",
                value: values
            });
        }

        return params;
    }

    function CloseModal() {
        var src = urlParams["src"];
        if (src == "RTE") {
            CloseRadWindow();
        } else if (src == "FieldEditor") {
            window.top.dialogClose("");
        }
    }

    function CloseModalAndReturnValue() {
        var src = urlParams["src"];
        if (src == "RTE") {
            CloseRadWindow(jQuery(".result-items .selected").html());
        } else if (src == "FieldEditor") {
            var value = jQuery(".result-items .selected img").attr("id");
            window.returnValue = value;
            window.top.returnValue = value;
            window.top.dialogClose();
        }
    }

    //get results for the first load
    if (jQuery(imageSearchForm).length)
        RunQuery();
});

//closes the search modal and passes value back to the RTE
function CloseRadWindow(value) {

    var radWindow;

    if (window.radWindow)
        radWindow = window.radWindow;
    else if (window.frameElement && window.frameElement.radWindow)
        radWindow = window.frameElement.radWindow;
    else
        window.close();

    radWindow.Close(value);
}

var urlParams;
(window.onpopstate = function () {
    var match,
        pl = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query = window.location.search.substring(1);

    urlParams = {};
    while (match = search.exec(query))
        urlParams[decode(match[1])] = decode(match[2]);
})();