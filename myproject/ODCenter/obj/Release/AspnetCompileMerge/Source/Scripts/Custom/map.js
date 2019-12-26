//var Map = window.Map = Map || {};
//(function () {
var InsButtom = Map.InsButtom = function () {
    this.defaultAnchor = BMAP_ANCHOR_BOTTOM_RIGHT;
    this.defaultOffset = new BMap.Size(10, 10);
}

InsButtom.prototype = new BMap.Control();

InsButtom.prototype.initialize = function (map) {
    var div = document.createElement("div");
    div.id = "ins-btn";
    div.appendChild(Common.createElement("span", null, null, "列表"));
    div.appendChild(Common.createElement("i", null, null, "<em></em>"));
    div.onclick = function (e) {
        if ($("#ins-list").is(":visible")) {
            $("#ins-list").hide();
            map.enableScrollWheelZoom();
        }
        else {
            $("#ins-list").height($("#map-content").height() * 0.75);
            $("#ins-list").show();
            map.disableScrollWheelZoom();
        }
    }
    $("#map-control").append(div);
    //map.getContainer().appendChild(div);
    return div;
}

var InsList = Map.InsList = function () {
    this.defaultAnchor = BMAP_ANCHOR_BOTTOM_RIGHT;
    this.defaultOffset = new BMap.Size(10, 36);
}

InsList.prototype = new BMap.Control();

InsList.prototype.initialize = function (map) {
    var div = Common.createElement("div", null, "ins-list");
    div.appendChild(Common.createElement("div", null, "ins-title", "机构列表"));
    div.appendChild(Common.createElement("div", null, "ins-box", "&#10006;", function () { $("#ins-list").hide(); map.enableScrollWheelZoom(); }));
    div.appendChild(Common.createElement("div", null, "ins-body"));
    $("#map-control").append(div);
    map.addEventListener("click", function () {
        $("#ins-list").hide();
        map.enableScrollWheelZoom();
    })
    return div;
}
//})();