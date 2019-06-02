jQuery.noConflict();

//train all groups
jQuery(document).ready(function () {
    var trainAllGroupsForm = ".train-all-groups-form";
    
    var dbValue = jQuery(trainAllGroupsForm + " #db").attr("value");
    var groupIdsValue = jQuery(trainAllGroupsForm + " #groupIds").attr("value");
    
    jQuery(trainAllGroupsForm).hide();
    jQuery(".progress-indicator").show();
    jQuery(".result-display").show();

    var groups = JSON.parse(groupIdsValue);
    var keys = [], name;
    for (name in groups) {
        if (!Object.prototype.hasOwnProperty.call(groups, name))
            continue;

        keys.push(name);
    }
    var groupsFinished = 0;
    var groupsSucceeded = 0;

    var timer;
    CheckStatus();

    function CheckStatus() {
        
        clearInterval(timer);

        var iid = keys[groupsFinished];
        var gid = groups[iid];

        jQuery.post(jQuery(trainAllGroupsForm).attr("action"), { id: iid, db: dbValue, groupId: gid })
        .done(function (result) 
        {
            //notstarted, running, succeeded, failed
            var status = result.status.toLocaleLowerCase();
            if (status === "succeeded")
                groupsSucceeded++;

            if (status === "succeeded" || status === "failed") {
                groupsFinished++;
        
                if (result.message != null)
                    jQuery(".result-message").append("<div>" + gid + " " + status + " - " + result.message + "</div>");
                else 
                    jQuery(".result-message").append("<div>" + gid + " " + status + "</div>");
            }
        
            jQuery(".result-display").text("Training " + groupsFinished + " of " + keys.length + " finished");
            
            if (groupsFinished === keys.length) {
                jQuery(".progress-indicator").hide();
                return;
            }
        
            timer = setInterval(CheckStatus, 1000);
        });
    }
});