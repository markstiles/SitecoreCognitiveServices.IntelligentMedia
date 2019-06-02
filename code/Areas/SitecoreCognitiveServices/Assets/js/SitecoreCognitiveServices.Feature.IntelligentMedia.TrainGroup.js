jQuery.noConflict();

//train group
jQuery(document).ready(function () {
    var trainGroupForm = ".train-group-form";
    
    var idValue = jQuery(trainGroupForm + " #id").attr("value");
    var dbValue = jQuery(trainGroupForm + " #db").attr("value");
    var groupIdValue = jQuery(trainGroupForm + " #groupId").attr("value");
    
    jQuery(trainGroupForm).hide();
    jQuery(".progress-indicator").show();
    jQuery(".result-display").show();

    var timer;
    CheckStatus();

    function CheckStatus() {

        clearInterval(timer);

        jQuery.post(jQuery(trainGroupForm).attr("action"), { id:idValue, db:dbValue, groupId: groupIdValue })
        .done(function (result) {

            jQuery(".result-display").text("Training " + result.status);
            
            //notstarted, running, succeeded, failed
            var status = result.status.toLocaleLowerCase();
            if (result.message != null)
                jQuery(".result-message").text(result.message);

            if (status === "succeeded" || status === "failed") {
                jQuery(".progress-indicator").hide();
                return;
            }
            
            timer = setInterval(CheckStatus, 1000);
        });
    };
});