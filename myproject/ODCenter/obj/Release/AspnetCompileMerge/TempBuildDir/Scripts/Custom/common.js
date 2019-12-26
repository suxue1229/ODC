var Navbar = {
    set: function () {
        if (document.getElementById("nav_bar_top") != null) {
            $.ajax({
                url: domain + "Nav",
                type: "post",
                dataType: "json",
                success: function (data) {
                    clearInterval(inthdl);
                    var tophtml = "";
                    var sidehtml = "";
                    $.each(data, function (main_idx, main_dat) {
                        if (controller != null && main_dat.controller == controller && group != null && group.length == 0) {
                            tophtml += "<li class='active head-main'><a href='" + domain + main_dat.controller.toLowerCase() + "'>" + main_dat.name + "</a></li>";
                            sidehtml += "<a href='" + domain + main_dat.controller.toLowerCase() + "' class='list-group-item active side-main'><span class='glyphicon " + main_dat.icon + "'></span>" + main_dat.name + "</a>";
                        }
                        else {
                            tophtml += "<li class='head-main'><a href='" + domain + main_dat.controller.toLowerCase() + "'>" + main_dat.name + "</a></li>";
                            sidehtml += "<a href='" + domain + main_dat.controller.toLowerCase() + "' class='list-group-item side-main'><span class='glyphicon " + main_dat.icon + "'></span>" + main_dat.name + "</a>";
                        }
                        $.each(main_dat.subs, function (sub_idx, sub_dat) {
                            if (group != null && sub_dat.id == group && controller != null && main_dat.controller == controller) {
                                tophtml += "<li class='active head-sub'><a href='" + domain + main_dat.controller.toLowerCase() + "/group/" + sub_dat.id + "'>" + sub_dat.name + "</a></li>";
                                sidehtml += "<a href='" + domain + main_dat.controller.toLowerCase() + "/group/" + sub_dat.id + "' class='list-group-item active side-sub'>" + sub_dat.name + "</a>";
                            }
                            else {
                                tophtml += "<li class='head-sub'><a href='" + domain + main_dat.controller.toLowerCase() + "/group/" + sub_dat.id + "'>" + sub_dat.name + "</a></li>";
                                sidehtml += "<a href='" + domain + main_dat.controller.toLowerCase() + "/group/" + sub_dat.id + "' class='list-group-item side-sub'>" + sub_dat.name + "</a>";
                            }
                        })
                    });
                    $("#nav_bar_top").html(tophtml);
                    $("#nav_bar_side").html(sidehtml);
                }
            });
        }
    }
}

var Color = {
    Good: "lightgreen",
    Slow: "lightgray",
    Poor: "darkgray",
    Dead: "dimgray",
    Warn: "orange",
    Erro: "red",
}

$("#router_query").click(function () {
    var idstr = $("#router_id").val();
    var id = Number(idstr);
    if (idstr.length == 0 || idstr.length > 4 || isNaN(id)) {
        alert("请输入正确的DTU ID尾号");
        if (isNaN(id)) {
            $("#router_id").val("");
        }
        $("#router_id").focus();
    }
    else {
        Router.linkQuery(id);
    }
});

$("#mode_lite").click(function () {
    $.cookie("mode", "");
    window.location.reload();
});

$("#mode_full").click(function () {
    $.cookie("mode", "full");
    window.location.reload();
});

$(document).on("click", ".account-invite-create", function () {
    $.ajax({
        url: domain + "api/user/invite/",
        type: "put",
        dataType: "json",
        success: function (data) {
            if (data.status == 0) {
                alert("已创建注册授权：\n  授权编码: " + data.data.Id + "\n  生成时间: " + data.data.Issued + "\n  过期时间: " + data.data.Expired)
                window.location.reload();
            }
        }
    });
});

$(document).on("click", ".account-invite-delete", function () {
    $.ajax({
        url: domain + "api/user/invite/" + $(this).data("invite-id"),
        type: "delete",
        dataType: "json",
        success: function (data) {
            if (data.status == 0) {
                window.location.reload();
            }
        }
    });
});

$(document).on("click", ".router-link-disconnect", function () {
    $.ajax({
        url: domain + "router/close/" + $(this).data("link-id"),
        type: "post",
        dataType: "json",
        success: function (data) {
            window.location.href = domain + "router";
        }
    });
});

var Common = {
    pad: function (num, n) {
        var len = num.toString().length;
        while (len < n) {
            num = "0" + num;
            len++;
        }
        return num;
    },

    inArray: function (array, element) {
        if (array == null) {
            return false;
        }
        for (i = 0; i < array.length && array[i] != element; i++);
        return !(i == array.length);
    },

    selectAll: function (except) {
        var elms = document.getElementsByTagName("*");
        $.each(elms, function (idx, elm) {
            if (elm.type == "checkbox" && !Common.inArray(except, elm.id)) {
                elm.checked = true;
            }
        });
    },

    selectNone: function (except) {
        var elms = document.getElementsByTagName("*");
        $.each(elms, function (idx, elm) {
            if (elm.type == "checkbox" && !Common.inArray(except, elm.id)) {
                elm.checked = false;
            }
        });
    },

    columns: function () {
        var width = document.body.clientWidth;
        return width > 1360 ? 5 :
            width > 1120 ? 4 :
            width > 880 ? 3 :
            width > 640 ? 2 :
            width > 480 ? 1 :
            width > 240 ? 2 : 1;
    },

    units: function (grp) {
        var cols = Common.columns();
        if (grp != null && grp.length > 0) {
            return cols == 1 ? 20 :
            cols == 2 ? 16 :
            cols == 3 ? 21 :
            cols == 4 ? 24 :
            cols == 5 ? 25 : 25;
        }
        else {
            return cols == 1 ? 5 :
                cols == 2 ? 8 :
                cols == 3 ? 9 :
                cols == 4 ? 8 :
                cols == 5 ? 10 : 10;
        }
    },

    formula: function (equ) {
        return equ;
    },

    initUI: function (type) {
        $("#nav_bar_top,.table-side").css("display", type == "Station" ? "none" : "");
    },

    clearInterval: function () {
        for (var i = 0; i < 99999; i++) {
            clearInterval(i);
        }
    },

    createElement: function (tag, clazz, id, html, fun) {
        var element = document.createElement(tag);
        if (clazz != null) {
            element.className = clazz
        }
        if (id != null) {
            element.id = id;
        }
        if (html != null) {
            element.innerHTML = html;
        }
        if (fun != null) {
            element.onclick = fun;
        }
        return element;
    }
}

var DateTime = {
    toString: function (date) {
        return "" + Common.pad(date.getFullYear(), 4) + Common.pad(date.getMonth() + 1, 2) + Common.pad(date.getDate(), 2)
            + Common.pad(date.getHours(), 2) + Common.pad(date.getMinutes(), 2);
    },

    fromString: function (date) {
        return new Date(parseInt(date.substr(0, 4)), parseInt(date.substr(4, 2)) - 1, parseInt(date.substr(6, 2)),
            parseInt(date.substr(8, 2)), parseInt(date.substr(10, 2)));
    },

    totalSeconds: function (date) {
        return Math.floor(new Date(date).getTime() / 1000);
    }
}

var Home = {
    login: function () {
        var form = document.forms["home_login"];
        $("#errortip").html("<div style='color:green;'>正在进行登录验证，请稍后...</div>");
        $.ajax({
            url: domain + "Account/Assert",
            type: "post",
            dataType: "json",
            data: "__RequestVerificationToken=" + form.elements["__RequestVerificationToken"].value + "&Email=" + form.elements["Email"].value + "&Password=" + form.elements["Password"].value,
            success: function (data) {
                if (data.State != null && data.State == "Success") {
                    $("#errortip").html("");
                    document.forms[0].submit();
                }
                else {
                    $("#errortip").html("<div style='color:red;'>" + data.Errors + "</div>");
                }
            }
        });
    }
}

var Account = {
    toggle: function (id) {
        $.ajax({
            url: domain + "Account/Switch/",
            type: "post",
            dataType: "json",
            data: "institute=" + id,
            success: function (data) {
                if (data.message != null) {
                    alert(data.message);
                }
                else {
                    window.location.href = domain + "Monitor";
                }
            }
        });
    }
}

var Map = {
    insarr: [],

    init: function () {
        map = new BMap.Map("map-content");
        map.setMapType(BMAP_NORMAL_MAP);
        map.centerAndZoom(new BMap.Point(105, 35), 5);
        map.enableScrollWheelZoom();
        map.enableContinuousZoom();
        //map.addControl(new BMap.NavigationControl());
        map.addControl(new BMap.ScaleControl());
        map.addControl(new BMap.MapTypeControl());
        map.addControl(new InsButtom());
        map.addControl(new InsList());
        Map.loadList();
        //setInterval(Map.loadList, 1500);
    },

    loadList: function () {
        $.ajax({
            url: domain + "Map/List/",
            type: "post",
            dataType: "json",
            success: function (data) {
                Map.insarr = [];
                if (data.status == 0) {
                    $.each(data.results, function (idx, ins) {
                        if (ins.longitude != null && ins.latitude != null) {
                            ins.convert = false;
                            Map.insarr.push(ins);
                        }
                    });
                }
                Map.locConv();
            }
        });
    },

    locConv: function () {
        var arr = [];
        for (var i = 0; i < Map.insarr.length && arr.length < 10; i++) {
            if (!Map.insarr[i].convert) {
                arr.push(new BMap.Point(Map.insarr[i].longitude, Map.insarr[i].latitude));
            }
        }
        if (arr.length == 0) {
            Map.render();
        } else {
            new BMap.Convertor().translate(arr, 1, 5, function (data) {
                if (data.status == 0) {
                    $.each(data.points, function (idx, pt) {
                        for (var i = 0; i < Map.insarr.length; i++) {
                            if (!Map.insarr[i].convert) {
                                Map.insarr[i].longitude = pt.lng;
                                Map.insarr[i].latitude = pt.lat;
                                Map.insarr[i].convert = true;
                                break;
                            }
                        }
                    });
                    Map.locConv();
                }
                else {
                    console.log("[ERROR][LOCONV]" + JSON.stringify(data));
                    Map.render();
                }
            });
        }
    },

    render: function () {
        var markers = [];
        $("#ins-body").html("");
        $.each(Map.insarr, function (idx, ins) {
            (function (ins) {
                var img = "factory-off.png";
                if (ins.type == 10) {
                    img = "station-off.png";
                }
                var icon = new BMap.Icon(domain + "images/map/" + img, new BMap.Size(48, 48), { imageSize: new BMap.Size(48, 48) });
                var marker = new BMap.Marker(new BMap.Point(ins.longitude, ins.latitude), { icon: icon });
                marker.setTitle(ins.name);
                //marker.setLabel(new BMap.Label(ins.name, { offset: new BMap.Size(20, -10) }));
                marker.addEventListener("click", function (e) {
                    var p = e.target;
                    var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
                    var html = "<h4 style='margin:0 0 5px 0;padding:0.2em 0'>" + ins.name + "</h4>";
                    html += "<p>" + ins.address + "</p>";
                    html += "<div class='btn btn-default btn-sm' id='btn-map-marker' style='position:absolute;right:0;bottom:0;' onclick='Map.toggle()' data-id='" + ins.id + "'>";
                    html += "<span class='fa fa-play' style='color:green;'></span></div>";
                    var infoWindow = new BMap.InfoWindow(html, { width: 250, height: 80 });
                    map.openInfoWindow(infoWindow, point);
                });
                $("#ins-body").append(Common.createElement("div", "ins-item", null, ins.name + "<br><div class='description'>" + ins.address + "</div>", function () {
                    map.centerAndZoom(new BMap.Point(ins.longitude, ins.latitude), 19);
                    marker.setLabel(new BMap.Label(ins.name, { offset: new BMap.Size(38, 0) }));
                    if (map.width <= 2 * $("#ins-list").width()) {
                        $("#ins-list").hide();
                        map.enableScrollWheelZoom();
                    }
                    try {
                        for (key in marker) {
                            if ($(marker[key]).hasClass("BMap_noprint")) {
                                $(marker[key]).trigger("click");
                                break;
                            }
                        }
                    }
                    catch (e) { }
                }));
                //marker.addEventListener("mouseover", function () {
                //    alert(JSON.stringify(ins));
                //    marker.setLabel(new BMap.Label(ins.name, { offset: new BMap.Size(20, -10) }))
                //});
                //marker.addEventListener("mouseout", function () {
                //    marker.setLabel()
                //});
                markers.push(marker);
            })(ins);
        });
        map.clearOverlays();
        new BMapLib.MarkerClusterer(map, { markers: markers });
    },

    toggle: function () {
        Account.toggle($("#btn-map-marker").data("id"));
    },
}

var Sensor = {
    showEdit: function (id) {
        $.ajax({
            url: domain + "Sensor/Detail/" + id,
            type: "post",
            dataType: "json",
            success: function (data) {
                var html = "<div id='sen_value_edit' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
                html += "<div class='modal-dialog'>";
                html += "<div class='modal-content'>";
                html += "<div class='modal-header'>";
                html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
                html += "<h4 class='modal-title' id='modallabel'>设置" + data.name + "的数值</h4>";
                html += "</div>";
                html += "<div class='modal-body'><div class='form-horizontal'>";
                html += "<input type='hidden' id='sen_set_id' value='" + id + "'>"
                html += "<div class='form-group'><div class='col-sm-4 control-label'>新数值：</div><input type='number' class='col-sm-8 form-control' id='sen_set_value' value='" + data.value + "'></div>";
                html += "</div></div>";
                html += "<div class='modal-footer'>";
                html += "<a id='sen_edit_save' class='btn btn-primary'>设置</a>";
                html += "<a class='btn btn-default' data-dismiss='modal'>关闭</a>";
                html += "</div></div></div></div>";
                $("#sen_edit_panel").html(html);
                $("#sen_edit_save").click(function () {
                    var val = $("#sen_set_value").val();
                    if (val != null && val.length > 0) {
                        $.ajax({
                            url: domain + "Sensor/Set/" + $("#sen_set_id").val(),
                            type: "post",
                            dataType: "json",
                            data: "value=" + val,
                            success: function (res) {
                                if (res.result != "ok") {
                                    alert(res.msg);
                                }
                                $("#sen_value_edit").modal("hide");
                            }
                        });
                    }
                })
                $("#sen_value_edit").modal("show");
            }
        });
    },

    updateList: function () {
        var type = $("#DatType").val();
        $("#SenType option").each(function () {
            if ($(this).val() != "Undefined") {
                if (type == "BOOL") {
                    $(this).attr("hidden", $(this).val().substring(0, 7) != "Status_");
                }
                else {
                    $(this).attr("hidden", $(this).val().substring(0, 7) == "Status_");
                }
            }
            if ($("#SenType").val() == $(this).val() && $(this).attr("hidden")) {
                $("#SenType").val("Undefined");
            }
        })
        $("#Inverse").parent().parent().attr("hidden", $("#SenType").val().substring(0, 7) != "Status_");
        $("#Precise").parent().parent().attr("hidden", type == "BOOL");
    }
}

var Client = {

}

var Router = {
    loadList: function () {
        var html = "<table class='table'><tr><th>连接ID</th><th>创建时间</th><th>最后活跃</th><th>PLC ID</th><th>DTU ID</th><th>客户端</th><th></th></tr>";
        $.ajax({
            url: domain + "router/link",
            type: "post",
            dataType: "json",
            success: function (data) {
                $.each(data, function (idx, dat) {
                    html += "<tr><td><a href='" + domain + "router/conn/" + dat.id + "'>" + dat.id.substring(0, 8) + "...</a></td><td>" + dat.time + "</td><td>" + dat.active + "</td><td>" + dat.remote.plc + "</td><td>";
                    html += dat.remote.dtu + "</td><td>" + (dat.remote.name != null ? dat.remote.name : "") + "</td><td>";
                    html += "<a class='btn btn-manage router-link-disconnect' href='javascript:void(0);' data-link-id='" + dat.id + "' title='断开连接'><span class='fa fa-chain-broken'></span></a></td></tr>";
                });
                html += "</table>";
                $("#link_panel").html(html);
            },
            error: function (err) {
                html += "</table>";
                $("#link_panel").html(html);
            }
        });
    },

    loadLink: function () {
        $.ajax({
            url: domain + "router/link/" + linkid,
            type: "post",
            dataType: "json",
            success: function (data) {
                if (data.status == null || data.status != "ok") {
                    Router.linkDead();
                    return;
                }
                var html = Router.linkTable("网络连接", { text: "断开连接", clazz: "router-link-disconnect", link_id: data.id },
                                    [{ name: "ID", text: data.id },
                                    { name: "远程端IP", text: data.ip },
                                    { name: "创建时间", text: data.time },
                                    { name: "最后活跃", text: data.active },
                                    { name: "客户端ID", text: data.remote.client },
                                    { name: "客户端名称", text: data.remote.name }]);
                html += Router.linkTable("流量统计", null,
                    [{ name: "发送包", text: data.txpack },
                    { name: "发送字节", text: data.txbyte },
                    { name: "接收包", text: data.rxpack },
                    { name: "接收字节", text: data.rxbyte },
                    { name: "无效字节", text: data.rxdrop }]);
                $("#link_panel").html(html);
                html = Router.linkTable("远程端", { text: "查看数据", link: detailurl + "/" + linkid },
                    [{ name: "PLC ID", text: data.remote.plc, group: "status", type: "number", index: 0, id: "plcid_set", link: "javascript:void(0);", tip: "设置远程端PLC ID" },
                    { name: "DTU ID", text: data.remote.dtu },
                    { name: "启动时间", text: data.remote.boot },
                    { name: "系统时间", text: data.remote.time, group: "status", type: "datetime", index: 4, id: "time_set", link: "javascript:void(0);", tip: "设置远程端系统时间" },
                    { name: "系统版本", text: data.remote.version }]);
                html += Router.linkTable("流量统计", { text: "复位计数", group: "func", data: "20=0", id: "flow_reset" },
                    [{ name: "发送包", text: data.remote.txpack },
                    { name: "发送失败", text: data.remote.txfail },
                    { name: "发送字节", text: data.remote.txbyte },
                    { name: "接收包", text: data.remote.rxpack },
                    { name: "接收失败", text: data.remote.rxfail },
                    { name: "接收字节", text: data.remote.rxbyte }]);
                html += Router.linkTable("GPS信息", { text: "立即同步", group: "func", data: "7=0", id: "gps_sync" },
                    [{ name: "经度", text: data.remote.gpslon },
                    { name: "纬度", text: data.remote.gpslat },
                    { name: "时间", text: data.remote.gpstime }]);
                html += Router.linkTable("同步间隔(s)", { text: "恢复默认", group: "stat", data: "32=0&33=0&34=0", id: "span_reset" },
                    [{ name: "数据同步", text: data.remote.datspan, group: "status", type: "number", index: 32, id: "datspan_set", link: "javascript:void(0);", tip: "设置数据同步间隔" },
                    { name: "状态同步", text: data.remote.staspan, group: "status", type: "number", index: 33, id: "staspan_set", link: "javascript:void(0);", tip: "设置状态同步间隔" },
                    { name: "GPS同步", text: data.remote.gpsspan, group: "status", type: "number", index: 34, id: "gpsspan_set", link: "javascript:void(0);", tip: "设置GPS同步间隔" }]);
                $("#remote_panel").html(html);
                $("#plcid_set, #time_set, #datspan_set, #staspan_set, #gpsspan_set").click(function () {
                    //{ title: $(this).attr("title").slice(2), name: $(this).attr("name"), group: $(this).attr("xgroup"),
                    //type: $(this).attr("xtype"), index: $(this).attr("xindex"), value: $(this).text() };
                    var html = "<div id='edit_dialog' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
                    html += "<div class='modal-dialog'>";
                    html += "<div class='modal-content'>";
                    html += "<div class='modal-header'>";
                    html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
                    html += "<h4 class='modal-title' id='modallabel'>" + $(this).attr("title").slice(2) + "</h4>";
                    html += "</div>";
                    html += "<div id='edit_body' class='modal-body'><form class='form-horizontal'>";
                    html += "<div class='form-group'><label class='col-md-3 control-label'>原" + $(this).attr("name") + "</label><div class='col-md-9'>";
                    html += "<input class='form-control text-box single-line' type='text' readonly='readonly' value='" + $(this).text() + "' /></div></div>";
                    html += "<div class='form-group'><label class='col-md-3 control-label'>新" + $(this).attr("name") + "</label><div class='col-md-9'>";
                    html += "<input class='form-control text-box single-line' id='set_value' xindex='" + $(this).attr("xindex") + "' type='" + $(this).attr("xtype") + "' value='' /></div></div>";
                    html += "</form></div>";
                    html += "<div class='modal-footer'>";
                    if ($(this).attr("xtype") == "datetime") {
                        html += "<a id='btn_now' class='btn btn-default'>快速校时</a>";
                    }
                    html += "<a id='btn_set' class='btn btn-primary'>设置</a>";
                    html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
                    html += "</div></div></div></div>";
                    $("#pop_panel").html(html);
                    $("#btn_now").click(function () {
                        $.ajax({
                            url: domain + "router/stat/" + linkid,
                            type: "post",
                            dataType: "json",
                            data: $("#set_value").attr("xindex") + "=0",
                            success: function (data) {
                                if (data.status == "ok") {
                                    alert("操作成功，请稍等以完成更新");
                                } else {
                                    alert("连接已关闭或出现未知错误，请稍后重试");
                                }
                            }
                        });
                        $("#edit_dialog").modal("hide");
                    });
                    $("#btn_set").click(function () {
                        var val = $("#set_value").val();
                        if (val == null || val.length == 0) {
                            alert("无效设置，请检查并重新输入");
                            return;
                        }
                        if ($("#set_value").attr("type") == "datetime") {
                            val = DateTime.totalSeconds(val);
                        }
                        $.ajax({
                            url: domain + "router/stat/" + linkid,
                            type: "post",
                            dataType: "json",
                            data: $("#set_value").attr("xindex") + "=" + val,
                            success: function (data) {
                                if (data.status == "ok") {
                                    alert("操作成功，请稍等以完成更新");
                                } else {
                                    alert("连接已关闭或出现未知错误，请稍后重试");
                                }
                            }
                        });
                        $("#edit_dialog").modal("hide");
                    });
                    $("#edit_dialog").modal("show");
                })
                $("#flow_reset, #gps_sync, #span_reset").click(function () {
                    $.ajax({
                        url: domain + "router/" + $(this).attr("xgroup") + "/" + linkid,
                        type: "post",
                        dataType: "json",
                        data: $(this).attr("xdata"),
                        success: function (data) {
                            if (data.status == "ok") {
                                alert("操作成功，请稍等以完成更新");
                            } else {
                                alert("连接已关闭或出现未知错误，请稍后重试");
                            }
                        }
                    });
                })
            }
        });
    },

    linkTable: function (title, opt, list) {
        var html = "<div class='col-sm-6'><div style='padding-bottom:40px'><div style='float:left;font-size:large;font-weight:bold'>" + title + "</div>";
        if (opt != null) {
            html += "<div style='float:left;padding-left:50px'><a class='btn btn-default"
            if (opt.clazz != null) {
                html += " " + opt.clazz;
            }
            html += "'";
            if (opt.id != null) {
                html += " id='" + opt.id + "'"
            }
            if (opt.group != null) {
                html += " xgroup='" + opt.group + "'"
            }
            if (opt.data != null) {
                html += " xdata='" + opt.data + "'"
            }
            if (opt.link != null) {
                html += " href='" + opt.link + "'"
            }
            if (opt.link_id != null) {
                html += " data-link-id='" + opt.link_id + "'";
            }
            html += ">" + opt.text + "</a></div>";
        }
        html += "</div><table class='table'>";
        var first = true;
        $.each(list, function (idx, dat) {
            html += "<tr><td";
            if (first) {
                html += " class='col-xs-5'";
                first = false;
            }
            html += ">" + dat.name + "</td><td>";
            if (dat.text != null) {
                if (dat.link != null) {
                    html += "<a";
                    if (dat.id != null) {
                        html += " id='" + dat.id + "'";
                    }
                    if (dat.tip != null) {
                        html += " title='点击" + dat.tip + "'";
                    }
                    if (dat.group != null) {
                        html += " xgroup='" + dat.group + "'";
                    }
                    if (dat.type != null) {
                        html += " xtype='" + dat.type + "'";
                    }
                    if (dat.index != null) {
                        html += " xindex='" + dat.index + "'";
                    }
                    html += " name='" + dat.name + "' href='" + dat.link + "'>" + dat.text + "</a>";
                } else {
                    html += dat.text;
                }
            }
            html += "</td></tr>";
        })
        html += "</table></div>";
        return html;
    },

    loadData: function () {
        $.ajax({
            url: domain + "router/link/" + linkid,
            type: "post",
            dataType: "json",
            success: function (data) {
                if (data.status == null || data.status != "ok") {
                    Router.linkDead();
                    return;
                }
                var html = Router.dataTable("数据项", null, data.data);
                $("#data_panel").html(html);
                var queue = [];
                $.each(data.queue.data, function (idx, dat) {
                    queue.push({ id: dat.id, type: "数据", name: dat.name, value: dat.value });
                });
                $.each(data.queue.stat, function (idx, dat) {
                    queue.push({ id: dat.id, type: "状态", name: dat.name, value: dat.value });
                });
                $.each(data.queue.func, function (idx, dat) {
                    queue.push({ id: dat.id, type: "功能", name: dat.name, value: dat.value });
                });
                html = Router.dataTable("发送队列", null, queue);
                $("#queue_panel").html(html);
            }
        });
    },

    dataTable: function (title, opt, list) {
        var html = "<div class='col-md-12'><div style='padding-bottom:40px'><div style='float:left;font-size:large;font-weight:bold'>" + title + "</div>";
        if (opt != null) {
            html += "<div style='float:right;'><a class='btn btn-default'";
            if (opt.id != null) {
                html += " id='" + opt.id + "'"
            }
            if (opt.link != null) {
                html += " href='" + opt.link + "'"
            }
            html += ">" + opt.text + "</a></div>";
        }
        html += "</div><table class='table'>";
        html += "<tr><th class='col-xs-3'>ID</th><th class='col-xs-3'>类型</th><th class='col-xs-3'>名称</th><th>数值</th></tr>";
        $.each(list, function (idx, dat) {
            html += "<tr><td>" + dat.id + "</td><td>" + dat.type + "</td><td>" + dat.name + "</td><td>" + dat.value + "</td></tr>";
        })
        html += "</table></div>";
        return html;
    },

    linkDead: function () {
        clearInterval(reload);
        var html = "<div id='redirect_dialog' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>连接已关闭</h4>";
        html += "</div>";
        html += "<div id='redirect_message' class='modal-body'></div>";
        html += "</div></div></div>";
        $("#pop_panel").html(html);
        Router.downCount(3, true);
    },

    downCount: function (count, first) {
        setTimeout(function () {
            if (first) {
                $("#redirect_dialog").modal('show');
            }
            if (count > 0) {
                $("#redirect_message").html("页面将在" + count + "秒后跳转至列表...");
                Router.downCount(count - 1, false);
            } else {
                location.href = domain + "router";
            }
        }, 1000);
    },

    setData: function () {

    },

    setStat: function () {

    },

    setFunc: function () {

    },

    linkQuery: function (id) {
        $.post(domain + "open/router/query", { query: id }, function (data) {
            var html = "<table class='table' style='max-width:520px;margin-left:auto;margin-right:auto;'>";
            html += "<tr><th>DTU ID</th><th>连接时间</th><th>最后活跃</th></tr>"
            if (data.status == 0) {
                $.each(data.result, function (idx, dat) {
                    html += "<tr><td><a href='" + domain + "open/router/detail/" + dat.id + "'>" + dat.remote.dtu + "</a>";
                    html += "</td><td>" + dat.time + "</td><td>" + dat.active + "</td></tr>"
                })
            }
            html += "</table>";
            $("#router_result").html(html);
        });
    },

    linkInfo: function () {
        $.post(domain + "open/router/query", { id: conn_id }, function (data) {
            var html = "";
            if (data.status == 0) {
                html = "<div class='col-md-6 col-xs-12'><div style='font-size:150%;'>连接信息</div><table class='table'>"
                html += "<tr><td>连接ID</td><td>" + data.result.id + "</td></tr>";
                html += "<tr><td>远程端IP</td><td>" + data.result.ip + "</td></tr>";
                html += "<tr><td>创建时间</td><td>" + data.result.time + "</td></tr>";
                html += "<tr><td>最后活跃</td><td>" + data.result.active + "</td></tr>";
                html += "<tr><td>客户端ID</td><td>" + data.result.remote.client + "</td></tr>";
                html += "<tr><td>客户端名称</td><td>" + data.result.remote.name + "</td></tr>";
                html += "<tr><td>PLC ID</td><td>" + data.result.remote.plc + "</td></tr>";
                html += "<tr><td>DTU IP</td><td>" + data.result.remote.dtu + "</td></tr>";
                html += "<tr><td>启动时间</td><td>" + data.result.remote.boot + "</td></tr>";
                html += "<tr><td>系统时间</td><td>" + data.result.remote.time + "</td></tr>";
                html += "<tr><td>系统版本</td><td>" + data.result.remote.version + "</td></tr>";
                html += "<tr><td>GPS位置</td><td>" + data.result.remote.gpslon + ", " + data.result.remote.gpslat + "</td></tr>";
                html += "<tr><td>GPS时间</td><td>" + data.result.remote.gpstime + "</td></tr>";
                html += "</table></div>";
                html += "<div class='col-md-6 col-xs-12'><div style='font-size:150%;'>远程端信息</div><table class='table'><tr><th>编号</th><th>名称</th><th>类型</th><th>数值</th></tr>"
                $.each(data.result.data, function (idx, dat) {
                    html += "<tr><td>" + dat.id + "</td><td>" + dat.name + "</td><td>" + dat.type + "</td><td>" + dat.value + "</td></tr>";
                });
                html += "</table></div>";
            }
            else {
                html = "<div style='text-align:center;'><h4>连接不存在或发生错误</h4></div>";
            }
            $("#router_link").html(html);
        });
    }
}

var Monitor = {
    updateImage: function () {
        $.ajax({
            url: domain + "Sensor/Group/",
            type: "post",
            dataType: "json",
            data: "limit=1000",
            success: function (data) {
                var srvtim = new Date(data.time);
                if (data.groups != null) {
                    $.each(data.groups, function (grp_idx, grp_dat) {
                        if (grp_dat.devices != null && grp_dat.devices.length > 0) {
                            $.each(grp_dat.devices, function (idx, dev) {
                                if (document.getElementById(dev.id + "_img")) {
                                    var tid = document.getElementById(dev.id + "_img");
                                    var mat = dev.status.match(/{(.*)}(.*)/);
                                    if (mat == null) {
                                        tid.innerHTML = "<label>" + dev.status + "</label>";
                                    }
                                    else {
                                        tid.innerHTML = "<label style=color:" + mat[1] + ";'>" + mat[2] + "</label>";
                                    }
                                }
                            })
                        }
                        if (grp_dat.sensors != null && grp_dat.sensors.length > 0) {
                            $.each(grp_dat.sensors, function (idx, dat) {
                                if (document.getElementById(dat.id + "_img")) {
                                    var tid = document.getElementById(dat.id + "_img");
                                    var timspan = srvtim - new Date(dat.time);
                                    if (timspan > 30000) {
                                        tid.innerHTML = "<label>#####</label>";
                                    }
                                    else {
                                        tid.innerHTML = "<label>" + dat.value.substring(0, 6) + "</label>";
                                    }
                                }
                            });
                        }
                    });
                }
            }
        });
    },

    loadList: function () {
        $.ajax({
            url: domain + "Sensor/Group/" + group,
            type: "post",
            dataType: "json",
            data: "limit=" + Common.units(group) + "&offset=" + offset,
            success: function (data) {
                var html = "";
                var srvtim = new Date(data.time);
                Common.initUI(data.type);
                if (data.type == "Station") {
                    $.each(data.groups, function (gidx, gdat) {
                        html += "<div class='frame_header col-xs-12'>";
                        html += "<label style='font-size:120%;font-weight:normal;padding-top:5px;'>" + gdat.name + "</label>";
                        $.each(gdat.devices, function (didx, ddat) {
                            html += "<div class='col-xs-12'>";
                            html += "<label><a href='" + domain + "manage/detail/" + ddat.id + "'>" + ddat.name + "</a></label>";
                            var mat = ddat.status.match(/{(.*)}(.*)/);
                            if (mat == null) {
                                html += "<label style='float:right;'>" + ddat.status + "</label>";
                            }
                            else {
                                html += "<label style='float:right;color:" + mat[1] + ";'>" + mat[2] + "</label>";
                            }
                            $.each(ddat.sensors, function (sidx, sdat) {
                                html += "<div class='col-xs-12' style='padding-right:0;color:dimgray;'>";
                                html += "<label><a href='" + domain + "monitor/detail/" + sdat.id + "'>" + sdat.name + "</a></label>";
                                html += "<label style='float:right;'>" + sdat.value;
                                if (sdat.unit) {
                                    html += " " + sdat.unit;
                                }
                                html += "</label></div>";
                            });
                            html += "</div>";
                        });
                        $.each(gdat.sensors, function (sidx, sdat) {
                            html += "<div class='col-xs-12'>";
                            html += "<label><a href='" + domain + "monitor/detail/" + sdat.id + "'>" + sdat.name + "</a></label>";
                            html += "<label style='float:right;'>" + sdat.value;
                            if (sdat.unit) {
                                html += " " + sdat.unit;
                            }
                            html += "</label></div>";
                        });
                        html += "</div>";
                    });
                    $("#list_panel").html(html);
                }
                else {
                    if (data.groups != null) {
                        $.each(data.groups, function (grp_idx, grp_dat) {
                            offset = grp_dat.offset;
                            if (grp_dat.sensors != null && grp_dat.sensors.length > 0) {
                                html += "<div class='frame_header col-xs-12'><label style='font-size:120%;font-weight:normal;padding-top:5px;'>" + grp_dat.name + "</label>";
                                if ((group == null || group.length == 0) && grp_dat.count > grp_dat.sensors.length) {
                                    html += "<a class='btn' style='float:right' href='#' onclick=\"javascript:window.location.href='" + domain + "monitor/group/" + grp_dat.id + "';\">更多</a>";
                                }
                                html += "</div><div class='col-xs-12'>";
                                $.each(grp_dat.sensors, function (idx, dat) {
                                    html += "<div class='sensor_frame'>";
                                    var timspan = srvtim - new Date(dat.time);
                                    if (dat.edit != null && dat.edit == true) {
                                        timspan = 0;
                                        html += "<button type='button' class='btn btn-default' style='position:absolute;top:8px;right:8px;width:20px;height:20px;' onclick='Sensor.showEdit(\"" + dat.id;
                                        html += "\")'><span class='glyphicon glyphicon-pencil' style='position:absolute;top:2px;left:6px;'></span></button>";
                                    }
                                    if (timspan > 30000) {
                                        html += "<span class='glyphicon glyphicon-link' style='position:absolute;top:10px;right:10px;'></span>";
                                    }
                                    html += "<button type='button' class='sensor_btn'";
                                    if (timspan > 30000) {
                                        html += " style='background-color:" + Color.Dead + ";' onclick='Monitor.showDetail(this.id)' id='" + dat.id + "'>" + dat.name + "<br />#####";
                                    }
                                    else if (timspan > 20000) {
                                        html += " style='background-color:" + Color.Poor + ";' onclick='Monitor.showDetail(this.id)' id='" + dat.id + "'>" + dat.name + "<br />" + dat.value;
                                    }
                                    else if (timspan > 10000) {
                                        html += " style='background-color:" + Color.Slow + ";' onclick='Monitor.showDetail(this.id)' id='" + dat.id + "'>" + dat.name + "<br />" + dat.value;
                                    }
                                    else {
                                        html += " style='background-color:" + Color.Good + ";' onclick='Monitor.showDetail(this.id)' id='" + dat.id + "'>" + dat.name + "<br />" + dat.value;
                                    }
                                    if (dat.unit != null && dat.unit.length > 0) {
                                        html += "(" + dat.unit + ")";
                                    }
                                    html += "</button></div>";
                                });
                                html += "</div>";
                                if (group != null && group.length > 0 && grp_dat.count > grp_dat.sensors.length) {
                                    html += "<div class='col-xs-12'><nav style='text-align:center;'><ul class='pagination' style='margin:0;'>";
                                    var highlight = Math.floor(grp_dat.offset / grp_dat.limit) + 1;
                                    for (var i = 1; i <= Math.ceil(grp_dat.count / grp_dat.limit) ; i++) {
                                        html += "<li";
                                        if (i == highlight) {
                                            html += " class='active'";
                                        };
                                        html += " style='padding:0;line-height:1;'><a href='#' onclick=\"javascript:offset=Common.units('" + grp_dat.id + "')*" + (i - 1) + ";Monitor.loadList();\">" + i + "</a></li>";
                                    }
                                    html += "</ul></nav></div>";
                                }
                            }
                        });
                        $("#list_panel").html(html);
                    }
                }
            }
        });
    },

    showDetail: function (id) {
        window.location.href = domain + "Monitor/Detail/".toLowerCase() + id;
    },

    loadDetail: function () {
        $.ajax({
            url: domain + "Sensor/Detail/" + id,
            type: "post",
            dataType: "json",
            success: function (data) {
                Common.initUI(data.ins.type);
                var html = "";
                var timespan = new Date(data.srvtime) - new Date(data.time);
                //html += "<div class='col-md-4 detail_value'>唯一标识: " + data.id + "</div>";
                html += "<div class='col-sm-4 detail_value'>仪表名称: " + data.name + "</div>";
                html += "<div class='col-sm-4 detail_value'>所属机构: " + data.ins.name + "</div>";
                html += "<div class='col-sm-4 detail_value'>实时数值: ";
                if (data.edit != null && data.edit == true) {
                    timespan = 0;
                }
                if (timespan > 30000) {
                    html += "#####";
                }
                else {
                    html += data.value;
                }
                if (data.unit != null && data.unit.length > 0) {
                    html += "(" + data.unit + ")";
                }
                if (data.edit != null && data.edit == true) {
                    html += "<button type='button' class='btn btn-link' style='padding:0px;text-indent:3px;height:18px;width:24px;' onclick='Sensor.showEdit(\"" + data.id;
                    html += "\")'><span class='glyphicon glyphicon-md  glyphicon-pencil'></span></button>"
                }
                html += "</div>";
                $("#detail_panel").html(html);
            }
        });
    },

    loadChart: function (dif) {
        if (dif != null) {
            zoomlevel = zoomlevel + dif;
            //if (chart != null) {
            //    chart.stop();
            //}
            chart_dat = null;
        }
        if (chart_dat == null) {
            document.getElementById("loading_state").style.color = "orange";
            $("#loading_state").html("正在加载历史数据...");
            setTimeout(Monitor.resetLoading, 1500);
            $.ajax({
                url: domain + "Sensor/Track/" + id + "?level=" + zoomlevel + "&time=" + DateTime.toString(new Date()),
                type: "post",
                dataType: "json",
                success: function (data) {
                    document.getElementById("loading_state").style.color = "green";
                    $("#loading_state").html("加载完成!");
                    setTimeout(Monitor.resetLoading, 500);
                    if (data.level != null && data.level != zoomlevel) {
                        zoomlevel = data.level;
                    }
                    var zoomnames = new Array("即时视图", "小时视图", "日视图", "周视图", "月视图", "年视图");
                    document.getElementById("chart_zoomout").disabled = zoomlevel >= 5;
                    $("#chart_zoomout").attr("title", zoomnames[Math.min(5, Math.max(0, zoomlevel + 1))]);
                    document.getElementById("chart_zoomin").disabled = zoomlevel <= 0;
                    $("#chart_zoomin").attr("title", zoomnames[Math.min(5, Math.max(0, zoomlevel - 1))]);
                    $("#chart_refresh").attr("title", "刷新" + zoomnames[Math.min(5, Math.max(0, zoomlevel))]);
                    chart_dat = {
                        labels: [],
                        datasets: [
                            {
                                label: "History Data",
                                fillColor: "rgba(100,100,100,0.2)",
                                strokeColor: "rgba(50,50,50,1)",
                                pointColor: "rgba(50,50,50,1)",
                                pointStrokeColor: "#fff",
                                pointHighlightFill: "#fff",
                                pointHighlightStroke: "rgba(0,0,0,1)",
                                data: []
                            }
                        ]
                    }
                    var minval = 0, avgval = 0, count = 0, maxval = 0, mintime, maxtime;
                    if (data.his != null) {
                        var tiphtml = "";
                        $.each(data.his, function (idx, dat) {
                            var time = new Date(dat.time);
                            if (count == 0) {
                                mintime = maxtime = time;
                                minval = avgval = maxval = dat.value;
                            }
                            else {
                                mintime = Math.min(mintime, time);
                                maxtime = Math.max(maxtime, time);
                                minval = Math.min(minval, dat.value);
                                maxval = Math.max(maxval, dat.value);
                                avgval = avgval + dat.value;
                            }
                            count = count + 1;
                            if (zoomlevel == 1 || zoomlevel == 2) {
                                tiphtml = Common.pad(time.getFullYear(), 4) + "年" + Common.pad(time.getMonth() + 1, 2) + "月" + Common.pad(time.getDate(), 2) + "日";
                                chart_dat.labels.push(Common.pad(time.getHours(), 2) + "h");
                            }
                            else if (zoomlevel == 3) {
                                tiphtml = Common.pad(time.getFullYear(), 4) + "年";
                                chart_dat.labels.push(Common.pad(time.getMonth() + 1, 2) + "-" + Common.pad(time.getDate(), 2));
                            }
                            else if (zoomlevel == 4) {
                                chart_dat.labels.push(Common.pad(time.getFullYear(), 4) + "-" + Common.pad(time.getMonth() + 1, 2));
                            }
                            else if (zoomlevel == 5) {
                                chart_dat.labels.push(Common.pad(time.getFullYear(), 4));
                            }
                            else {
                                tiphtml = Common.pad(time.getFullYear(), 4) + "年" + Common.pad(time.getMonth() + 1, 2) + "月" + Common.pad(time.getDate(), 2) + "日";
                                chart_dat.labels.push(Common.pad(time.getHours(), 2) + ":" + Common.pad(time.getMinutes(), 2));
                            }
                            chart_dat.datasets[0].data.push(dat.value);
                        })
                        $("#chart_tip").html(tiphtml);
                        Monitor.drawChart();
                        if (count > 0) {
                            var timstr = "";
                            if (count == 1) {
                                timstr += "一次";
                            }
                            else {
                                var msspan = (maxtime - mintime) / 1000, days = 0, hours = 0, mins = 0, secs = 0;
                                secs = Math.floor(msspan * 10) / 10; msspan = msspan / 60;
                                mins = Math.floor(msspan * 10) / 10; msspan = msspan / 60;
                                hours = Math.floor(msspan * 10) / 10; msspan = msspan / 24;
                                days = Math.floor(msspan * 10) / 10;

                                if (days >= 1) {
                                    timstr += days + "天";
                                }
                                else if (hours >= 1) {
                                    timstr += hours + "小时";
                                }
                                else if (mins >= 1) {
                                    timstr += mins + "分钟";
                                }
                                else {
                                    timstr += secs + "秒";
                                }
                            }
                            avgval = avgval / count;
                            var sumhtml = "";
                            sumhtml += "<div class='col-xs-12 detail_value'>最近" + timstr + "的统计数据：</div>";
                            sumhtml += "<div class='col-sm-12 detail_value' style='text-indent:20px;'>最小值:" + minval.toPrecision(2) + "</div></div>";
                            sumhtml += "<div class='col-sm-12 detail_value' style='text-indent:20px;'>平均值:" + avgval.toPrecision(2) + "</div></div>";
                            sumhtml += "<div class='col-sm-12 detail_value' style='text-indent:20px;'>最大值:" + maxval.toPrecision(2) + "</div></div>";
                            $("#summary_panel").html(sumhtml);
                        }
                    }
                }
            });
        }
    },

    resetLoading: function () {
        $("#loading_state").html("");
    },

    drawChart: function () {
        var cvw = document.body.clientWidth;
        if (cvw >= 1200) {
            cvw = 1170;
        }
        else if (cvw >= 992) {
            cvw = 970;
        }
        else if (cvw >= 768) {
            cvw = 750;
        }
        if (cvw > 480) {
            cvw = cvw - 160;
        }
        cvw = cvw - 20;
        $("#chart_panel").attr("width", cvw);
        $("#chart_panel").attr("height", 0.48 * cvw);
        //if (chart) {
        //    chart.clear();
        //    chart.destroy();
        //}
        var canvas = $("#chart_panel").get(0);
        var ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        if (chart_dat != null && chart_dat.datasets[0].data.length > 0) {
            if (chart_dat.datasets[0].data.length > 1) {
                chart = new Chart(ctx, { type: 'line', data: chart_dat });
            }
            else {
                chart = new Chart(ctx, { type: 'bar', data: chart_dat });
            }
        }
    }
}

var Manage = {
    loadList: function () {
        $.ajax({
            url: domain + "Device/Group/" + group,
            type: "post",
            dataType: "json",
            data: "limit=" + Common.units(group) + "&offset=" + offset,
            success: function (data) {
                var html = "";
                var srvtim = new Date(data.time);
                if (data.groups != null) {
                    $.each(data.groups, function (grp_idx, grp_dat) {
                        offset = grp_dat.offset;
                        if (grp_dat.devices != null && grp_dat.devices.length > 0) {
                            html += "<div class='frame_header col-xs-12'><label style='font-size:120%;font-weight:normal;padding-top:5px;'>" + grp_dat.name + "</label>";
                            if ((group == null || group.length == 0) && grp_dat.count > grp_dat.devices.length) {
                                html += "<a class='btn' style='float:right' href='#' onclick=\"javascript:window.location.href='" + domain + "manage/group/" + grp_dat.id + "';\">更多</a>";
                            }
                            html += "</div>";
                            $.each(grp_dat.devices, function (idx, dat) {
                                html += "<div class='sensor_frame'>";
                                html += "<button type='button' class='sensor_btn' style='background-color:";
                                if (dat.status == null) {
                                    dat.status = "{orange}Unknown";
                                }
                                var mat = dat.status.match(/{(.*)}(.*)/);
                                html += (mat == null) ? "orange" : (mat[1] == "green") ? "lightgreen" : mat[1];
                                html += ";' onclick='Manage.showDetail(this.id)' id='" + dat.id + "'>" + dat.name + "<br />";
                                html += (mat == null) ? dat.status : mat[2];
                                html += "</button>";
                                html += "</div>";
                            });
                            if (group != null && group.length > 0 && grp_dat.count > grp_dat.devices.length) {
                                html += "<div class='col-md-12'><nav style='text-align:center;'><ul class='pagination' style='margin:0;'>";
                                var highlight = Math.floor(grp_dat.offset / grp_dat.limit) + 1;
                                for (var i = 1; i <= math.ceil(grp_dat.count / grp_dat.limit) ; i++) {
                                    html += "<li";
                                    if (i == highlight) {
                                        html += " class='active'";
                                    };
                                    html += " style='padding:0;line-height:1;'><a href='#' onclick=\"javascript:offset=Common.units('" + grp_dat.id + "')*" + (i - 1) + ";Manage.loadList();\">" + i + "</a></li>";
                                }
                                html += "</ul></nav></div>";
                            }
                        }
                    });
                    $("#list_panel").html(html);
                }
            }
        });
    },

    showDetail: function (id) {
        window.location.href = domain + "Manage/Detail/".toLowerCase() + id;
    },

    loadDetail: function () {
        $.ajax({
            url: domain + "Device/Detail/" + id,
            type: "post",
            dataType: "json",
            success: function (data) {
                Common.initUI(data.ins.type);
                var detail_html = "";
                var srvtim = new Date(data.srvtime);
                detail_html += "<div class='col-sm-4 detail_value'>设备名称: " + data.name + "</div>";
                detail_html += "<div class='col-sm-4 detail_value'>所属机构: " + data.ins.name + "</div>";
                detail_html += "<div class='col-sm-4 detail_value'>设备状态: ";
                if (data.status == null) {
                    data.status = "{orange}Unknown";
                }
                var mat = data.status.match(/{(.*)}(.*)/);
                detail_html += (mat == null) ? data.status : "<span style='color:" + mat[1] + ";'>" + mat[2] + "</span>";
                if (data.edit) {
                    detail_html += "<button class='btn btn-link' style='padding:0px;text-indent:3px;height:18px;width:24px;' id='" + data.id + "' data-toggle='tooltip' title='修改状态' onclick='Manage.editState(this.id)'><span class='glyphicon glyphicon-md glyphicon-pencil'></span></button>";
                }
                detail_html += "</div>";
                $("#detail_panel").html(detail_html);
                var sensor_html = "";
                var sensor_count = 0;
                if (data.sensors != null) {
                    $.each(data.sensors, function (idx, dat) {
                        sensor_count = sensor_count + 1;
                        if ((sensor_count % 2) == 0) {
                            sensor_html += "<div class='col-xs-12' style='border-radius:3px;padding-left:0;padding-right:0;background-color:lightgray;'>";
                        }
                        else {
                            sensor_html += "<div class='col-xs-12' style='border-radius:3px;padding-left:0;padding-right:0;background-color:whitesmoke;'>";
                        }
                        sensor_html += "<div class='col-xs-4'>" + dat.name + "</div>";
                        sensor_html += "<div class='col-xs-4'";
                        var timspan = srvtim - new Date(dat.time);
                        if (timspan > 30000) {
                            sensor_html += " style='color:red;'>#####"
                        }
                        else if (timspan > 20000) {
                            sensor_html += " style='color:orangered;'>" + dat.value;
                        }
                        else if (timspan > 10000) {
                            sensor_html += " style='color:orange;'>" + dat.value;
                        }
                        else {
                            sensor_html += " style='color:green;'>" + dat.value;
                        }
                        if (dat.unit != null && dat.unit.length > 0) {
                            sensor_html += "(" + dat.unit + ")";
                        }
                        sensor_html += "</div><div class='col-xs-4 text-center'><button class='btn btn-link' style='padding:0px;width:28px;' data-toggle='tooltip' title='仪表详情' onclick='Monitor.showDetail(this.id)' id='" + dat.id + "'><span class='glyphicon glyphicon-md glyphicon-info-sign'></span></button></div></div>";
                    });
                    $("#sensor_panel").html(sensor_html);
                }
            }
        });
    },

    editState: function (id) {
        $.ajax({
            url: domain + "Device/State/" + id,
            type: "get",
            dataType: "html",
            cache: false,
            success: function (data) {
                $("#summary_holder").html(data);
                var mat = document.forms["state_edit"].elements["Status"].value.match(/{(.*)}(.*)/);
                if (mat != null) {
                    switch (mat[1]) {
                        case "green":
                            Manage.toggleState("stat_ok");
                            break;
                        case "red":
                            Manage.toggleState("stat_err");
                            break;
                        default:
                            Manage.toggleState("stat_ukw");
                            break;
                    }
                    document.forms["state_edit"].elements["Status"].value = mat[2];
                }
                $("#editstate").modal('show');
            }
        });
    },

    toggleState: function (mark) {
        var statnames = new Array("stat_ok", "stat_ukw", "stat_err");
        statnames.forEach(function (value) {
            if (mark == value) {
                document.getElementById(value).className = "btn btn-default active";
            }
            else {
                document.getElementById(value).className = "btn btn-default";
            }
        });
    },

    setState: function (state) {
        switch (state.style.color) {
            case "green":
                Manage.toggleState("stat_ok");
                break;
            case "red":
                Manage.toggleState("stat_err");
                break;
            default:
                Manage.toggleState("stat_ukw");
                break;
        }
        document.forms["state_edit"].elements["Status"].value = state.innerHTML;
    },

    saveState: function () {
        var form = document.forms["state_edit"];
        var stpre = document.getElementById("stat_ok").className == "btn btn-default active" ? "green" :
            //document.getElementById("stat_ukw").className == "btn btn-default active"?"\o":
            document.getElementById("stat_err").className == "btn btn-default active" ? "red" : "orange";
        stpre = "{" + stpre + "}";
        $.ajax({
            url: domain + "Device/State/" + id,
            type: "post",
            dataType: "json",
            data: "__RequestVerificationToken=" + form.elements["__RequestVerificationToken"].value + "&Id=" + form.elements["Id"].value + "&Name=" + form.elements["Name"].value + "&Status=" + stpre + form.elements["Status"].value,
            success: function (data) {
                if (data.result == "ok") {
                    $("#editstate").modal('hide');
                }
                else {
                    $("#errortip").html(data.msg);
                }
                Manage.loadDetail();
            }
        });
    },

    newTrack: function () {
        $.ajax({
            url: domain + "Device/Track/Create/" + id,
            type: "get",
            dataType: "html",
            cache: false,
            success: function (data) {
                $("#summary_holder").html(data);
                $("#createtrack").modal('show');
            }
        });
    },

    createTrack: function () {
        var form = document.forms["track_create"];
        $.ajax({
            url: domain + "Device/Track/Create/",
            type: "post",
            dataType: "json",
            data: "__RequestVerificationToken=" + form.elements["__RequestVerificationToken"].value + "&HisTime=" + form.elements["HisTime"].value + "&DeviceId=" + form.elements["DeviceId"].value + "&Description=" + form.elements["Description"].value,
            success: function (data) {
                if (data.State == "Success") {
                    $("#createtrack").modal('hide');
                }
                else {
                    $("#errortip").html(data.Errors);
                }
                Manage.loadTrack();
            }
        });
    },

    editTrack: function (trackid) {
        $.ajax({
            url: domain + "Device/Track/Edit/" + trackid,
            type: "get",
            dataType: "html",
            cache: false,
            success: function (data) {
                $("#summary_holder").html(data);
                $("#edittrack").modal('show');
            }
        });
    },

    saveTrack: function () {
        var form = document.forms["track_edit"];
        $.ajax({
            url: domain + "Device/Track/Edit/",
            type: "post",
            dataType: "json",
            data: "__RequestVerificationToken=" + form.elements["__RequestVerificationToken"].value + "&Id=" + form.elements["Id"].value + "&HisTime=" + form.elements["HisTime"].value + "&DeviceId=" + form.elements["DeviceId"].value + "&Description=" + form.elements["Description"].value,
            success: function (data) {
                if (data.State == "Success") {
                    $("#edittrack").modal('hide');
                }
                else {
                    $("#errortip").html(data.Errors);
                }
                Manage.loadTrack();
            }
        });
    },

    deleteTrack: function (trackid) {
        $.ajax({
            url: domain + "Device/Track/Delete/" + trackid,
            type: "get",
            dataType: "html",
            cache: false,
            success: function (data) {
                $("#summary_holder").html(data);
                $("#deletetrack").modal('show');
            }
        });
    },

    removeTrack: function () {
        var form = document.forms["track_delete"];
        $.ajax({
            url: domain + "Device/Track/Delete/",
            type: "post",
            dataType: "json",
            data: "__RequestVerificationToken=" + form.elements["__RequestVerificationToken"].value + "&Id=" + form.elements["Id"].value,
            success: function (data) {
                $("#deletetrack").modal('hide');
                if (data.State != "Success") {
                    alert(data.Errors);
                }
                Manage.loadTrack();
            }
        });
    },

    loadTrack: function () {
        document.getElementById("loading_state").style.color = "orange";
        $("#loading_state").html("正在加载设备记录...");
        setTimeout(Manage.resetLoading, 1500);
        $.ajax({
            url: domain + "Device/Track/Fetch/" + id,
            type: "post",
            dataType: "json",
            success: function (data) {
                document.getElementById("loading_state").style.color = "green";
                $("#loading_state").html("加载完成!");
                var html = "";
                html += "<div class='col-sm-4 col-md-3 col-lg-3 detail_value'>记录时间</div>";
                html += "<div class='col-sm-4 col-md-5 col-lg-6 detail_value'>操作描述</div>";
                html += "<div class='col-sm-4 col-md-4 col-lg-3 detail_value'>操作员</div>";
                $.each(data, function (idx, his) {
                    html += "<div class='col-sm-4 col-md-3 col-lg-3 detail_value'>" + his.time + "</div>";
                    html += "<div class='col-sm-4 col-md-5 col-lg-6 detail_value'>" + his.log + "</div>";
                    html += "<div class='col-sm-4 col-md-4 col-lg-3 detail_value'>" + his.oper;
                    html += "<button class='btn btn-link' style='padding:0px;text-indent:3px;height:18px;width:24px;' id='" + his.id + "' data-toggle='tooltip' title='修改纪录' onclick='Manage.editTrack(this.id)'><span class='glyphicon glyphicon-md glyphicon-pencil'></span></button>";
                    html += "<button class='btn btn-link' style='padding:0px;text-indent:3px;height:18px;width:24px;' id='" + his.id + "' data-toggle='tooltip' title='删除纪录' onclick='Manage.deleteTrack(this.id)'><span class='glyphicon glyphicon-md glyphicon-trash'></span></button>";
                    html += "</div>";
                })
                $("#summary_panel").html(html);
                setTimeout(Manage.resetLoading, 500);
            }
        });
    },

    resetLoading: function () {
        $("#loading_state").html("");
    }
}

var Report = {
    pageEdit: function () {
        var html = "";
        html += "<div id='data_page' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>" + name + "的页面设置</h4>";
        html += "</div>";
        html += "<div class='modal-body'><form class='form-horizontal'>";
        $.each([["PageWidth", "页面宽度"], ["PageHeight", "页面高度"], ["MarginTop", "上边距"], ["MarginBottom", "下边距"], ["MarginLeft", "左边距"], ["MarginRight", "右边距"]], function (idx, nam) {
            html += "<div class='form-group'><label class='col-md-3 control-label'>" + nam[1] + "</label><div class='col-md-9'>";
            html += "<input class='form-control text-box single-line' id='page_" + nam[0] + "' type='text' value='' /></div></div>";
        });
        html += "</form></div>";
        html += "<div class='modal-footer'>";
        html += "<a id='btn_page_save' class='btn btn-primary'>保存</a>";
        html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
        html += "</div></div></div></div>";
        $("#page_panel").html(html);
        $("#btn_page_save").click(function () {
            var page = "";
            $.each(["PageWidth", "PageHeight", "MarginTop", "MarginBottom", "MarginLeft", "MarginRight"], function (idx, nam) {
                var ctrl = document.getElementById("page_" + nam);
                if (ctrl != null && ctrl.value != null && ctrl.value.length > 0) {
                    var val = parseFloat(ctrl.value);
                    if (ctrl.value.indexOf("cm") > 0) {
                        val *= 0.3937;
                    }
                    page += nam + "=" + val.toFixed(2) + "in;";
                }
            });
            $("#data_page").modal('hide');
            $("#PageInfo").val(page);
        });
        $.each($("#PageInfo").val().split(";"), function (idx, itm) {
            if (itm != null & itm.length > 0) {
                var props = itm.split("=");
                $("#page_" + props[0]).val(props[1]);
            }
        });
        $("#data_page").modal('show');
    },

    tempUpload: function () {
        var html = "";
        html += "<div id='temp_upload' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>报告模板上传</h4>";
        html += "</div>";
        html += "<div class='modal-body'><form id='file_upload_form' class='form-horizontal' action='" + domain + "Report/Upload/" + id + "' method='post' enctype='multipart/form-data'>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>选择模板文件</label><div class='col-md-9'>";
        html += "<input class='form-control text-box single-line' id='file_to_upload' type='file' accept='.rdlc' value='' /></div></div>";
        html += "</form></div>";
        html += "<div class='modal-footer'>";
        html += "<a id='btn_file_upload' class='btn btn-primary'>上传</a>";
        html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
        html += "</div></div></div></div>";
        html += "";
        $("#file_panel").html(html);
        $("#btn_file_upload").click(function () {
            var form = $("form#file_upload_form");
            var formData = new FormData();
            var file = $("#file_to_upload").get(0).files[0];
            if (file == null) {
                alert("请选择报告模板文件。");
                return;
            }
            if (file.type != "application/xml" || file.name.substring(file.name.lastIndexOf(".") + 1).toLowerCase() != "rdlc") {
                alert("所选文件不是有效的报告模板文件，请重新选择。");
                return;
            }
            formData.append("upload", file, file.name);
            $.ajax({
                url: form.attr("action"),
                method: form.attr("method"),
                data: formData,
                enctype: 'multipart/form-data',
                success: function (data) {
                    if (data.result != null && data.result == true) {
                        alert("报告模板上传成功。");
                        $("#temp_upload").modal('hide');
                    }
                    else if (data.msg != null) {
                        alert("报告模板上传失败:" + data.msg);
                    }
                    if (data.file != null) {
                        $("#temp_file_status").val(data.file);
                    }
                },
                cache: false,
                contentType: false,
                processData: false
            });
        });
        $("#temp_upload").modal('show');
    },

    tempEdit: function () {
        try {
            temp = $.parseJSON($("#DataSource").val());
        }
        catch (exception) {
            temp = new Object();
            temp.Items = [];
        }
        var html = "";
        html += "<div id='data_source' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>" + name + " - 数据源</h4>";
        html += "</div>";
        html += "<div id='temp_cont_panel' class='modal-body'>";
        html += "</div>";
        html += "<div class='modal-footer'>";
        html += "<a id='btn_temp_add' class='btn btn-default'>新增</a>";
        html += "<a id='btn_temp_del' class='btn btn-default'>删除</a>";
        html += "<a id='btn_temp_edit' class='btn btn-default'>编辑</a>";
        html += "<a id='btn_temp_save' class='btn btn-primary'>保存</a>";
        html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
        html += "</div></div></div></div>";
        html += "";
        $("#temp_panel").html(html);
        $("#btn_temp_add").click(function () {
            item = new Object();
            item.Ids = [];
            Report.itemName();
        });
        $("#btn_temp_del").click(function () {
            $('input:checkbox').each(function () {
                if ($(this).attr('id').substring(0, 4) == "itm_" && $(this).is(':checked') == true) {
                    var itmid = $(this).attr('id').substring(4);
                    if (confirm("确认删除数据源" + itmid + "？")) {
                        var itms = [];
                        while (temp.Items.length > 0) {
                            var val = temp.Items.pop();
                            if (val.Name == itmid) {
                                break;
                            }
                            else {
                                itms.push(val);
                            }
                        }
                        while (itms.length > 0) {
                            temp.Items.push(itms.pop());
                        }
                    }
                }
            });
            Report.tempUpdate();
        });
        $("#btn_temp_edit").click(function () {
            $('input:checkbox').each(function () {
                if ($(this).attr('id').substring(0, 4) == "itm_" && $(this).is(':checked') == true) {
                    var itmid = $(this).attr('id').substring(4);
                    $.each(temp.Items, function () {
                        if (this.Name == itmid) {
                            item = new Object();
                            for (var elem in this) {
                                if (elem == 'Ids') {
                                    item.Ids = [];
                                    for (i = 0; i < this.Ids.length; i++) {
                                        item.Ids.push(this.Ids[i]);
                                    }
                                } else {
                                    item[elem] = this[elem];
                                }
                            }
                            item.Ori = this.Name;
                        }
                    });
                    return false;
                }
            });
            Report.itemName();
        });
        $("#btn_temp_save").click(function () {
            $("#DataSource").val(JSON.stringify(temp));
            $("#data_source").modal('hide');
        });
        $("#data_source").modal('show');
        Report.tempUpdate();
        if (ids == null) {
            Report.idsLoad();
        }
    },

    tempUpdate: function () {
        var html = "";
        html += "<table class='table'>";
        html += "<tr><th></th><th>Name</th><th>Count</th><th></th>";
        if (temp != null && temp.Items != null) {
            $.each(temp.Items, function (idx, itm) {
                html += "<tr><td><input type='checkbox' id='itm_" + itm.Name + "' /></td>";
                html += "<td>" + itm.Name + "</td>";
                html += "<td>" + itm.Ids.length + "</td></tr>";
            });
        }
        html += "</table>";
        $("#temp_cont_panel").html(html);
    },

    itemName: function () {
        $("#data_item").modal('hide');
        var html = "";
        html += "<div id='data_item' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>" + name + " - 新增数据源 - 名称</h4>";
        html += "</div>";
        html += "<div class='modal-body'><form class='form-horizontal'>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>数据名称</label><div class='col-md-9'>";
        html += "<input class='form-control text-box single-line' id='item_ItemName' type='text' value='' /></div></div>";
        html += "</form></div>";
        html += "<div class='modal-footer'>";
        html += "<a id='btn_item1_fw' class='btn btn-primary'>下一步</a>";
        html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
        html += "</div></div></div></div>";
        $("#item_panel").html(html);
        $("#btn_item1_fw").click(function () {
            item.Name = $("#item_ItemName").val().trim();
            if (item.Name == null || item.Name.length == 0) {
                alert("数据源名称无效，请重新输入。");
                return;
            }
            var isvalid = true;
            $.each(temp.Items, function () {
                if ((item.Ori == null && this.Name == item.Name) || (item.Ori != null && this.Name != item.Ori && this.Name == item.Name)) {
                    alert("数据源名称重复，请重新输入。");
                    isvalid = false;
                    return false;
                }
            })
            if (isvalid) {
                Report.itemSubs();
            }
        });
        $("#item_ItemName").val(item.Name);
        $("#data_item").modal('show');
    },

    itemSubs: function () {
        $("#data_item").modal('hide');
        var html = "";
        html += "<div id='data_item' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>" + name + " - 新增数据源 - 项列表</h4>";
        html += "</div>";
        html += "<div id='item_subs_panel' class='modal-body'>";
        html += "</div>";
        html += "<div class='modal-footer'>";
        html += "<a id='btn_item2_bw' class='btn btn-default'>上一步</a>";
        html += "<a id='btn_item2_add' class='btn btn-default'>增加项</a>";
        html += "<a id='btn_item2_del' class='btn btn-default'>删除项</a>";
        html += "<a id='btn_item2_fw' class='btn btn-primary'>下一步</a>";
        html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
        html += "</div></div></div></div>";
        $("#item_panel").html(html);
        var idsUpdate = function () {
            if (ids == null) {
                return;
            }
            var html = "";
            html += "<table class='table'>";
            html += "<tr><th></th><th>Group</th><th>Name</th>";
            $.each(ids, function () {
                html += "<tr><td><input type='checkbox' id='ids_" + this.Id + "' /></td>";
                html += "<td>" + this.Group + "</td>";
                html += "<td>" + this.Name + "</td></tr>";
            })
            html += "</table>";
            $("#subs_list_panel").html(html);
        };
        $("#btn_item2_bw").click(function () {
            Report.itemName();
        });
        $("#btn_item2_add").click(function () {
            var html = "";
            html += "<div id='item_subs' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
            html += "<div class='modal-dialog'>";
            html += "<div class='modal-content'>";
            html += "<div class='modal-header'>";
            html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
            html += "<h4 class='modal-title' id='modallabel'>添加项至列表</h4>";
            html += "</div>";
            html += "<div id='subs_list_panel' class='modal-body'>";
            html += "</div>";
            html += "<div class='modal-footer'>";
            html += "<a id='btn_add_ids' class='btn btn-primary'>添加</a>";
            html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
            html += "</div></div></div></div>";
            $("#subs_panel").html(html);
            if (ids == null) {
                Report.idsLoad();
            }
            idsUpdate();
            $("#btn_add_ids").click(function () {
                $('input:checkbox').each(function () {
                    if ($(this).attr('id').substring(0, 4) == "ids_" && $(this).is(':checked') == true) {
                        var id = $(this).attr('id').substring(4);
                        item.Ids.push(id);
                    }
                });
                Report.itemUpdate();
                $("#item_subs").modal('hide');
            })
            $("#item_subs").modal('show');
        });
        $("#btn_item2_del").click(function () {
            $('input:checkbox').each(function () {
                if ($(this).attr('id').substring(0, 4) == "sub_" && $(this).is(':checked') == true) {
                    var id = $(this).attr('id').substring(4);
                    var cache = [];
                    while (item.Ids.length > 0) {
                        var tid = item.Ids.pop();
                        if (tid != id) {
                            cache.push(tid);
                        }
                    }
                    while (cache.length > 0) {
                        item.Ids.push(cache.pop());
                    }
                }
            });
            Report.itemUpdate();
        });
        $("#btn_item2_fw").click(function () {
            Report.itemCfg();
        });
        $("#data_item").modal('show');
        Report.itemUpdate();
    },

    idsLoad: function () {
        $.ajax({
            url: domain + "Sensor/Group/",
            type: "post",
            dataType: "json",
            data: "limit=1000&ins=" + ins,
            success: function (data) {
                ids = [];
                if (data.groups != null) {
                    $.each(data.groups, function (grp_idx, grp_dat) {
                        if (grp_dat.sensors != null && grp_dat.sensors.length > 0) {
                            $.each(grp_dat.sensors, function (idx, dat) {
                                var id = new Object();
                                id.Group = grp_dat.name;
                                id.Id = dat.id;
                                id.Name = dat.name;
                                ids.push(id);
                            });
                        }
                    });
                }
            }
        });
    },

    itemUpdate: function () {
        if (ids == null) {
            Report.idsLoad();
        }

        var html = "";
        html += "<table class='table'>";
        html += "<tr><th></th><th>Group</th><th>Name</th>";
        if (item != null && item.Ids != null) {
            $.each(item.Ids, function (idx, itm) {
                html += "<tr><td><input type='checkbox' id='sub_" + itm + "' /></td>";
                var isvalid = false;
                $.each(ids, function () {
                    if (this.Id == itm) {
                        html += "<td>" + this.Group + "</td>";
                        html += "<td>" + this.Name + "</td></tr>";
                        isvalid = true;
                        return false;
                    }
                })
                if (!isvalid) {
                    html += "<td></td><td>" + itm + "</td></tr>";
                }
            });
        }
        html += "</table>";
        $("#item_subs_panel").html(html);
    },

    itemCfg: function () {
        $("#data_item").modal('hide');
        var lst = "";
        lst += "<option value='Hour'>一小时</option>";
        lst += "<option value='Day'>一天</option>";
        lst += "<option value='Week'>一周</option>";
        lst += "<option value='Month'>一个月</option>";
        lst += "<option value='Year'>一年</option>";
        var html = "";
        html += "<div id='data_item' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>" + name + " - 新增数据源 - 配置</h4>";
        html += "</div>";
        html += "<div class='modal-body'><form class='form-horizontal'>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>数据跨度</label><div class='col-md-9'>";
        html += "<select id='item_Span' class='form-control dropdown single-line'>" + lst + "</select></div></div>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>间隔精度</label><div class='col-md-9'>";
        html += "<select id='item_Level' class='form-control dropdown single-line'>" + lst + "</select></div></div>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>自然边界</label><div class='col-md-9'>";
        html += "<input class='form-control checkbox single-line' id='item_Slip' type='checkbox' value='' /></div></div>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>数量限制</label><div class='col-md-9'>";
        html += "<input class='form-control text-box single-line' id='item_Limit' type='number' value='' /></div></div>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>当前时间</label><div class='col-md-9'>";
        html += "<input class='form-control checkbox single-line' id='item_TimeCur' type='checkbox' checked='checked' /></div></div>";
        html += "<div class='form-group'><label class='col-md-3 control-label'>指定时间</label><div class='col-md-9'>";
        html += "<input class='form-control datetime single-line' id='item_TimeSet' type='datetime' value='' disabled='true' /></div></div>";
        html += "</form></div>";
        html += "<div class='modal-footer'>";
        html += "<a id='btn_item3_bw' class='btn btn-default'>上一步</a>";
        html += "<a id='btn_item3_fw' class='btn btn-primary'>保存</a>";
        html += "<a class='btn btn-default' data-dismiss='modal' style='float:left;'>关闭</a>";
        html += "</div></div></div></div>";
        $("#item_panel").html(html);
        $("#item_TimeCur").click(function () {
            if ($("#item_TimeCur").is(":checked")) {
                $("#item_TimeSet").attr("disabled", "disabled");
            }
            else {
                $("#item_TimeSet").removeAttr("disabled");
            }
        });
        var convDate = function (date) {
            return Common.pad(date.getFullYear(), 4) + "-" + Common.pad(date.getMonth() + 1, 2) + "-" + Common.pad(date.getDate(), 2)
                    + " " + Common.pad(date.getHours(), 2) + ":" + Common.pad(date.getMinutes(), 2) + ":" + Common.pad(date.getSeconds(), 2);
        };
        var rendeCfg = function () {
            if (item.Span != null) {
                $("#item_Span").val(item.Span);
            }
            if (item.Level != null) {
                $("#item_Level").val(item.Level);
            }
            if (item.Slip == null || item.Slip == true) {
                $("#item_Slip").attr("checked", true);
            }
            $("#item_Limit").val(0);
            if (item.Limit != null) {
                $("#item_Limit").val(item.Limit);
            }
            $("#item_TimeCur").attr("checked", true);
            $("#item_TimeSet").val("");
            $("#item_TimeSet").attr("disabled", "disabled");
            if (item.Time != null) {
                var date = new Date(item.Time.replace(/-/g, "/"));
                if (date != "Invalid Date") {
                    $("#item_TimeCur").attr("checked", false);
                    $("#item_TimeSet").val(convDate(date));
                    $("#item_TimeSet").removeAttr("disabled");
                }
            }
        };
        var parseCfg = function () {
            item.Span = $("#item_Span").val();
            item.Level = $("#item_Level").val();
            item.Slip = $("#item_Slip").is(":checked");
            var limit = $("#item_Limit").val().trim();
            item.Limit = 0;
            if (limit.length > 0) {
                item.Limit = limit * 1;
            }
            item.Time = null;
            if (!$("#item_TimeCur").is(":checked")) {
                var date = new Date($("#item_TimeSet").val().replace(/-/g, "/"));
                if (date != "Invalid Date") {
                    item.Time = convDate(date);
                }
            }
        };
        $("#btn_item3_bw").click(function () {
            parseCfg();
            Report.itemSubs();
        });
        $("#btn_item3_fw").click(function () {
            parseCfg();
            if (temp == null) {
                temp = new Object();
            }
            if (temp.Items == null) {
                temp.Items = [];
            }
            var itms = [];
            while (temp.Items.length > 0) {
                var val = temp.Items.pop();
                if (val.Name != item.Ori && val.Name != item.Name) {
                    itms.push(val);
                }
            }
            while (itms.length > 0) {
                temp.Items.push(itms.pop());
            }
            delete item.Ori;
            temp.Items.push(item);
            $("#data_item").modal('hide');
            Report.tempUpdate();
        });
        rendeCfg();
        $("#data_item").modal('show');
    },

    editTime: function (id) {
        var html = "<div id='time_edit' class='modal fade' role='dialog' aria-labelledby='modallabel' aria-hidden='true'>";
        html += "<div class='modal-dialog'>";
        html += "<div class='modal-content'>";
        html += "<div class='modal-header'>";
        html += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button>";
        html += "<h4 class='modal-title' id='modallabel'>设置报告时间</h4>";
        html += "</div>";
        html += "<div class='modal-body'>";
        html += "<select id='rpt_year'></select>年";
        html += "<select id='rpt_month'></select>月";
        html += "<select id='rpt_day'></select>日&nbsp;";
        html += "<select id='rpt_hour'></select>时";
        html += "<select id='rpt_minute'></select>分";
        html += "</div>";
        html += "<div class='modal-footer'>";
        html += "<a id='time_edit_clear' class='btn btn-default' style='float:left;'>清除</a>";
        html += "<a id='time_edit_set' class='btn btn-primary'>设置</a>";
        html += "<a class='btn btn-default' data-dismiss='modal'>关闭</a>";
        html += "</div></div></div></div>";
        $("#time_panel").html(html);
        $("#time_edit_clear").click(function () {
            if (timeid != null) {
                $(timeid).val("");
                if ($(timeid).hasClass("btn-link")) {
                    $(timeid).removeClass("btn-link");
                }
            }
            $("#time_edit").modal("hide");
        });
        $("#time_edit_set").click(function () {
            if (timeid != null) {
                var date = new Date($("#rpt_year").val(), $("#rpt_month").val() - 1,
                    $("#rpt_day").val(), $("#rpt_hour").val(), $("#rpt_minute").val());
                if (date != "Invalid Date") {
                    $(timeid).val(date.toString());
                    if (!$(timeid).hasClass("btn-link")) {
                        $(timeid).addClass("btn-link");
                    }
                }
                else {
                    if ($(timeid).hasClass("btn-link")) {
                        $(timeid).removeClass("btn-link");
                    }
                }
            }
            $("#time_edit").modal("hide");
        });
        html = "";
        for (var i = 2014; i <= new Date().getFullYear() ; i++) {
            html += "<option>" + i + "</option>";
        }
        $("#rpt_year").html(html);

        html = "";
        for (var i = 1; i <= 12 ; i++) {
            html += "<option>" + i + "</option>";
        }
        $("#rpt_month").html(html);

        html = "";
        for (var i = 1; i <= 31 ; i++) {
            html += "<option>" + i + "</option>";
        }
        $("#rpt_day").html(html);

        html = "";
        for (var i = 0; i <= 23 ; i++) {
            html += "<option>" + i + "</option>";
        }
        $("#rpt_hour").html(html);

        html = "";
        for (var i = 0; i <= 59 ; i++) {
            html += "<option>" + i + "</option>";
        }
        $("#rpt_minute").html(html);
        var date = new Date($(id).val());
        if (date == "Invalid Date") {
            date = new Date();
        }
        $("#rpt_year").val(date.getFullYear());
        $("#rpt_month").val(date.getMonth() + 1);
        $("#rpt_day").val(date.getDate());
        $("#rpt_hour").val(date.getHours());
        $("#rpt_minute").val(date.getMinutes());
        timeid = id;
        $("#time_edit").modal("show");
    },

    genReport: function (link) {
        var typ = $(link).attr("id").substring(0, 3);
        var rpt = $(link).attr("id").substring(4);
        var date = new Date($("#rpt_" + rpt).val());
        var param = "";
        if (date != "Invalid Date") {
            param = "?time=" + DateTime.toString(date);
        }
        if (typ == "rpv") {
            window.open(domain + "pdf/viewer.html?file=" + domain + "report/pdf/" + rpt + encodeURIComponent(param));
        }
        else if (typ == "rpp") {
            window.open(domain + "report/pdf/" + rpt + param);
        }
        else if (typ == "rpe") {
            window.open(domain + "report/excel/" + rpt + param);
        }
    }
}

var Vision = {

}