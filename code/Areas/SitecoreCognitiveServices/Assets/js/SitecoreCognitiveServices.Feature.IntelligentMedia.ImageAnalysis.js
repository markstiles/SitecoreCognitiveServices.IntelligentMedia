jQuery.noConflict();

//nav
jQuery(document).ready(function () {
    //toggles tabs based on nav clicks
    jQuery(".nav-btn")
        .click(function () {
            var selected = "selected";
            var tab = jQuery(this).attr("rel");
            jQuery(".nav-btn").removeClass(selected);
            jQuery(".tab-content").removeClass(selected);
            jQuery(".tab-content." + tab).addClass(selected);
            jQuery(this).addClass(selected);
        });
});

//analyze
jQuery(document).ready(function () {
    jQuery(".analyze-form button")
        .click(function (event) {
            jQuery(this).hide();
            jQuery(".analysis-warning").hide();
            jQuery(".progress-indicator").show();
        });

    // Global Variables
    var displayImage = jQuery(".image-analysis-wrapper img"); // rendered image
    var image = new Image(); // original image
    var $rects; // An array of all rectangles/face frame of the analyzed image
    var $origRectSpecs;  // An array of objects that contain the dimensions and positions of all the elements in $rect

    image.src = displayImage.attr("src");

    /**
     * Sets the global variables $rects and $origRectSpecs
     */
    function setConstants() {
        const wrapItems = ".image-analysis-wrapper .face-wrap, .image-analysis-wrapper .score-wrap, .image-analysis-wrapper .attribute-wrap, .image-analysis-wrapper .region-block, .image-analysis-wrapper .region-block .word-block .word-wrap";

        $rects = jQuery(".image-analysis-wrapper .rectangle");

        // Iterate over each rectangle and save the width, height, top position,
        // left position, closest stats block element, and position of the closest
        // stats block element to an object.  Each object is then added to the 
        // $origRectSpecs array for global use.
        $origRectSpecs = $rects.map(function () {
            closestWrapItems = jQuery(this).siblings(wrapItems);

            const stats = closestWrapItems.map(function () {
                return {
                    origStatTop: jQuery(this).position().top || parseInt(jQuery(this).css("top")),
                    origStatLeft: jQuery(this).position().left || parseInt(jQuery(this).css("left"))
                }
            })

            return {
                origRectWidth: jQuery(this).width(),
                origRectHeight: jQuery(this).height(),
                origRectTop: jQuery(this).position().top || parseInt(jQuery(this).css("top")), // if the rect is on a tab that is currently not displayed it has a position of 0, so this check gets the css instead so we don't lose the value
                origRectLeft: jQuery(this).position().left || parseInt(jQuery(this).css("left")),
                statBlock: closestWrapItems[0],
                statPosition: stats[0]
            }
        })
    }

    /**
     * Iterate over each rectangle saved in the global variable $rect and set the size and position
     * to scale with the rendered image.
     */
    function adjustFaceWrap() {
        const ratioWidth = displayImage.width() / image.width;
        const ratioHeight = displayImage.height() / image.height;

        $rects.map(function (i) {
            const { origRectWidth, origRectHeight, origRectTop, origRectLeft, statPosition, statBlock } = $origRectSpecs[i];
            jQuery(this).css({
                "width": `${origRectWidth * ratioWidth}px`,
                "height": `${origRectHeight * ratioHeight}px`,
                "top": `${origRectTop * ratioHeight}px`,
                "left": `${origRectLeft * ratioWidth}px`
            });

            if (statBlock) {
                jQuery(statBlock).css({
                    "top": `${statPosition.origStatTop * ratioHeight}px`,
                    "left": `${statPosition.origStatLeft * ratioWidth}px`
                });
            }
        })
    }

    image.onload = () => {
        setConstants();
        adjustFaceWrap();
    };

    jQuery(window).on("resize", function () { adjustFaceWrap() });
});