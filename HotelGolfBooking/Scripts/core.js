function getProvinList(value) {
    //var formdata = new FormData(); //FormData object
    //formdata.append("keyword", keyword);
    //formdata.append("location", location);
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/ListProvince/getList');
    xhr.send();
    var content = "";
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var news = '{"news":' + xhr.responseText + '}';
            var json_parsed = $.parseJSON(news);
            
            $("#provin").html("");
            for (var i = 0; i < json_parsed.news.length; i++) {
                if (json_parsed.news[i]) {
                    var name = json_parsed.news[i].provin;
                    //alert(name);
                    $("#provin").append("<option value='"+name+"'>"+name+"</option>");
                }
            }
            $("#provin").val(value);
            //alert(news);
        }
    }
}
function getDisList(provin,dis) {
    //alert("value=" + value);
    var formdata = new FormData(); //FormData object
    formdata.append("provin", provin);
    //formdata.append("location", location);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/ListProvince/getDisList');
    xhr.send(formdata);
    var content = "";
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var news = '{"news":' + xhr.responseText + '}';
            var json_parsed = $.parseJSON(news);

            $("#dis").html("");
            for (var i = 0; i < json_parsed.news.length; i++) {
                if (json_parsed.news[i]) {
                    var name = json_parsed.news[i].dis;
                    //alert(name);
                    $("#dis").append("<option value='" + name + "'>" + name + "</option>");
                }
            }
            $("#dis").val(dis);
            //alert(news);
        }
    }
}
function unicodeToNoMark(str) {
    if (str == null) return "";
    //return str;
    //input = input.toLowerCase();
    //var noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
    //var unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
    //var a_n = noMark.split(',');
    //var a_u = unicode.split(',');
    //for (var i = 0; i < a_n.length; i++) {
    //    input = input.replace(a_u[i],a_n[i]);
    //}
    str = removeSpecialCharater(str);
    str = str.replace(/\s/g, '-');
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    //str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
    str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1-
    str = str.replace(/^\-+|\-+$/g, "");//cắt bỏ ký tự - ở đầu và cuối chuỗi

    return str;

}
function unicodeToNoMarkCat(str) {
    if (str == null) return "";
    //return str;
    //input = input.toLowerCase();
    //var noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
    //var unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
    //var a_n = noMark.split(',');
    //var a_u = unicode.split(',');
    //for (var i = 0; i < a_n.length; i++) {
    //    input = input.replace(a_u[i],a_n[i]);
    //}
    str = removeSpecialCharater(str);
    str = str.replace(/\s/g, '-');
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    //str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
    str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1-
    str = str.replace(/^\-+|\-+$/g, "");//cắt bỏ ký tự - ở đầu và cuối chuỗi
    str = str.replace(/-/g, ""); //thay thế 2- thành 1-

    return str;

}
function removeSpecialCharater(input) {
    if (input == null) return "";
    //input = input.replace(/&quot;/g, '"');
    input = input.trim();
    input = input.replace(/\./g, "");
    //input = input.replace(/\,/g, "");
    input = input.replace(/\&/g, "");
    input = input.replace(/\'/g, "");
    input = input.replace(/\"/g, "");
    input = input.replace(/\;/g, "");
    input = input.replace(/\?/g, "");
    input = input.replace(/\!/g, "");
    input = input.replace(/\~/g, "");
    input = input.replace(/\*/g, "");
    input = input.replace(/\:/g, "");
    input = input.replace(/\"/g, "");
    input = input.replace("/", "");
    input = input.replace("%", "");
    input = input.replace("‘", "");
    input = input.replace("’", "");
    input = input.replace(/\"/g, "");
    input = input.replace("+", "");
    input = input.replace("“", "");
    input = input.replace("-", "_");
    input = input.replace("”", "");
    //input = input.replace(",", "");
    input = input.replace(/\,/g, "");
    //input = input.replace(".", "");

    return input;
    //.replace(",", "").replace("_", "").replace("'", "").replace("\"", "").replace(";", "").replace("”", "").replace(".", "");
}
function getDateId(sDate) {
    if (sDate==null || sDate=="") {return null;}
    sDate = sDate.replace(/\//g, "");
    //alert(sDate);
    sDate = sDate.substring(4, 8) + sDate.substring(2, 4) + sDate.substring(0, 2);
    return sDate;
}
function convertFromDateIdToDateString(sDate) {
    //sDate = sDate.replace(/\//g, "");
    //alert(sDate);
    sDate = sDate.substring(6, 8) + "/" + sDate.substring(4, 6) + "/" + sDate.substring(0, 4);
    return sDate;
}
function getAddressList(value) {
    //var formdata = new FormData(); //FormData object
    //formdata.append("keyword", keyword);
    //formdata.append("location", location);
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/ListAddress/getAddressList');
    xhr.send();
    var content = "";
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var news = '{"news":' + xhr.responseText + '}';
            var json_parsed = $.parseJSON(news);
            var preProvin = json_parsed.news[0].provin;
            //alert(news);
            $("#dis").html("<option selected=\"selected\" disabled=\"disabled\">Chọn</option>");
            $("#dis").append("<option value='" + json_parsed.news[0].provin + "' >" + json_parsed.news[0].provin + "</option>");
            for (var i = 0; i < json_parsed.news.length; i++) {
                if (json_parsed.news[i]) {
                    var name = json_parsed.news[i].dis;
                    //alert(name);
                    if (preProvin != json_parsed.news[i].provin) {                        
                        $("#dis").append("<option value='" + json_parsed.news[i].provin + "'>" + json_parsed.news[i].provin + "</option>");
                        preProvin = json_parsed.news[i].provin;
                    } 
                    //$("#dis").append("<option value='" + name + "'>" + name + "</option>");
                }
            }
            $("#dis").val(value);
            //alert(news);
        }
    }
}
function searchHotel() {
    var keyword = document.getElementById('hotelname').value;
    //alert(keyword);
    $('#hotelname').autocomplete({
        source: '/Hotels/getListHotel?keyword=' + keyword,
        select: function (event, ui) {
            //alert(ui.item.id);
            $(event.target).val(ui.item.value);
            //search();
            //$('#search_form').submit();
            return false;
        },
        minLength: 2
    });
}
function searchHotelAuto() {
    var keyword = document.getElementById('hotelnameauto').value;
    //alert(keyword);
    $('#hotelnameauto').autocomplete({
        source: '/Hotels/getListHotel?keyword=' + keyword,
        select: function (event, ui) {
            //alert(ui.item.id);
            $(event.target).val(ui.item.value);
            autosearchhotel(ui.item.value);
            return false;
        },
        minLength: 2
    });
}
function autosearchhotel(val){
    $("#labelautosearch").html("Đang tìm kiếm...<img src=\"/Images/loading.gif\" width=20 height=20>");
    var formdata = new FormData(); //FormData object
    formdata.append("keyword", val);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Hotels/getIdHotelByName');
    xhr.send(formdata);
    var content = "";
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var id = xhr.responseText;
            if (id != "-1") {
                $("#labelautosearch").html("Gõ tên khách sạn cần tìm..");
                window.open("/Hotels/Edit/"+id,"_blank");
            }
        }
    }
}
function searchGolf() {
    var keyword = document.getElementById('golfname').value;
    //alert(keyword);
    $('#golfname').autocomplete({
        source: '/Golf/getListGolf?keyword=' + keyword,
        select: function (event, ui) {
            $(event.target).val(ui.item.value);
            //search();
            //$('#search_form').submit();
            return false;
        },
        minLength: 2
    });
}
    function searchDis() {
        var keyword = document.getElementById('dis').value;
        //alert(keyword);
        $('#dis').autocomplete({
            source: '/ListAddress/getDisList?keyword=' + keyword,
            select: function (event, ui) {
                //alert(ui.item.id);
                $(event.target).val(ui.item.value);
                //search();
                //$('#search_form').submit();
                return false;
            },
            minLength: 2
        });
    }
    function getHotelListPromotion(fromdate,todate, idhotel,id) {
        var formdata = new FormData(); //FormData object
        formdata.append("fromdate", fromdate);
        formdata.append("todate", todate);
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelPromotion/getHotelPromotion');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                $("#promotionList_" + id).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].discount;
                        var tdate = convertFromDateIdToDateString("" + json_parsed.news[i].tdate + "");
                        var fdate = convertFromDateIdToDateString("" + json_parsed.news[i].fdate + "");
                        content += "<p><b>Khuyến mại " + name + "% từ "+fdate+" đến " + tdate + "</b></p>";
                    }
                }
                $("#promotionList_" + id).html(content);
                //$("#promotionList_" + idhotel).append();
            }
        }
    }
    function getGolfListPromotion(iddate, idgolf, id) {
        var formdata = new FormData(); //FormData object
        formdata.append("fdate", iddate);
        formdata.append("idgolf", idgolf);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/GolfPromotion/getGolfPromotion');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                $("#promotionList_" + id).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].discount;
                        var tdate = convertFromDateIdToDateString("" + json_parsed.news[i].tdate + "");
                        var fdate = convertFromDateIdToDateString("" + json_parsed.news[i].fdate + "");
                        content += "<p><b>Khuyến mại " + name + "% từ " + fdate + " đến " + tdate + "</b></p>";
                    }
                }
                $("#promotionList_" + id).html(content);
                //$("#promotionList_" + idhotel).append();
            }
        }
    }
    function hidePromotionOutOfThisDate(fromdate,todate, idhotel) {
        var formdata = new FormData(); //FormData object
        formdata.append("fromdate", fromdate);
        formdata.append("todate", todate);
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelPromotion/getHotelPromotion');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                if (json_parsed.news.length <= 0)
                {
                    $("#item_" + idhotel).hide();
                }            
            }
        }
    }
    function getGolfPromotion(idgolf,fdate) {
        var formdata = new FormData(); //FormData object
        formdata.append("idgolf", idgolf);
        formdata.append("fdate", fdate);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/GolfPromotion/getGolfPromotion');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                //$("#promotionList_" + idgolf).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].discount;
                        //var tdate = convertFromDateIdToDateString("" + json_parsed.news[i].tdate + "");
                        //var fdate = convertFromDateIdToDateString("" + json_parsed.news[i].fdate + "");
                        content = "<a style=\"margin-left:2px;\" href=\"/GolfPromotion/Details?id=" + json_parsed.news[i].id + "\" target=\"_blank\"><b>Chi tiết khuyến mại</b></a>";
                    
                    }
                }
                //alert(content);
                if (json_parsed.news.length > 0) {
                    $("#promotionList_" + idgolf).append(content);                
                }
                getGolfChouponList(idgolf,fdate);
                //$("#promotionList_" + idhotel).append();
            }
        }
    }
    function getGolfChouponList(idgolf, fdate) {
   
        var formdata = new FormData(); //FormData object
        formdata.append("id", idgolf);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/GolfChoupon/getChouponListGolf');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                //$("#promotionList_" + idgolf).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        content = "<a class=\"btn btn-info\" href=\"/GolfBooking/Contact?idgolf=" + idgolf + "&checkin=" + fdate + "&typebook=1\" target=\"_blank\"><b>Giá coupon " + formatCurrency(json_parsed.news[i].chouponprice) + "</b></a>";
                   
                    }
                }
                //alert(content);
                if (json_parsed.news.length > 0) $("#promotionList_" + idgolf).html(content);
                //$("#promotionList_" + idhotel).append();
            }
        }
    }
    function getGolfPrice(idgolf, dateid) {
        var formdata = new FormData(); //FormData object
        formdata.append("idgolf", idgolf);
        formdata.append("dateid", dateid);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/GolfPrice/getGolfPrice');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                $("#price_" + idgolf).html("");
                //for (var i = 0; i < json_parsed.news.length; i++) {
                //    if (json_parsed.news[i]) {
                //        var price = json_parsed.news[i].price;                    
                //        content += price;
                //    }
                //}
                if (json_parsed.news.length>0) $("#price_" + idgolf).html("<p>Giá trong tuần: <font color=#00B08F >" + formatCurrency(json_parsed.news[0].price) + " VNĐ</font></p><p>Cuối tuần/ngày lễ: <font color=#00B08F>" + formatCurrency(json_parsed.news[0].priceweekend) + " VNĐ</font></p>");
                //$("#promotionList_" + idhotel).append();
            }
        }
    }
    function getHotelListFacility(idhotel,type) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelFacilities/getHotelFacilities');
        xhr.send(formdata);
        var content = "";
        var space = ", ";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#facilityList_" + idhotel).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].facility;
                        if (i >= 5) { space = "..."; }
                        $("#facilityList_" + idhotel).append(name + space);
                        if (type==1 && i >= 5) break;
                    }
                }

                //alert(news);
            }
        }
    }
    function getHotelListFacilityBooking(idhotel, type) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelFacilities/getHotelFacilities');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#facilityList_" + idhotel).html("");
                content += "<tr>";
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].facility;
                        content += "<td><i class=\"fa fa-check-circle\"></i>" + name + "</td>";
                        if (type == 1 && i >= 5) break;
                        if ((i + 1) % 5 == 0) {
                            content += "</tr><tr>";
                        }
                    }
                }
                content += "</tr>";
                $("#facilityList_" + idhotel).html(content);
                //alert(news);
            }
        }
    }
    function getHotelListService(idhotel, type) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelService/getHotelServices');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#serviceList_" + idhotel).html("");
                content += "<tr>";
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].service;
                        content+="<td><i class=\"fa fa-check-circle\"></i>" + name + "</td>";
                        if (type == 1 && i >= 5) break;
                        if ((i+1) % 5 == 0) {
                            content+="</tr><tr>";
                        }
                    }
                }
                content += "</tr>";
                $("#serviceList_" + idhotel).html(content);
                //alert(news);
            }
        }
    }
    function getHotelChouponDetail(idhotel, idroom, price, fromdate, todate, invisibleprice, showprice) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        formdata.append("idroom", idroom);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelChoupon/getHotelChouponDetail');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                var chouponprice = 0;
                var id = 0;
                if (json_parsed.news.length > 0) {
                
                    if (json_parsed.news[0] && json_parsed.news[0].chouponprice) {
                        chouponprice = json_parsed.news[0].chouponprice;
                    }
                    id = json_parsed.news[0].id;
                }
                //var json_parsed = $.parseJSON(news);
                $("#choupon_" + idhotel).html("");
                //for (var i = 0; i < json_parsed.news.length; i++) {
                //if (json_parsed.news[i]) {
                //var name = json_parsed.news[i].chouponprice;
                    
                //break;
                //}
                //}
            
                if (chouponprice == 0) {
                    if (invisibleprice == 0 && showprice == 0 && price != 0 && price != null) $("#chouponRoom_" + idroom).html("<b class=pricedetail>" + formatCurrency(price) + " vnđ</b>");
                    if (invisibleprice == 1 || showprice == 1 || price == 0 || price == null) {
                        //$("#chouponRoom_" + idroom).html("<input type=\"button\" class=\"btn btn-info\" value=\"Click Để Lấy Giá\" onclick=\"getPrice("+idhotel+");\">");
                        $("#chouponRoom_" + idroom).html("<a href=\"/CustomerRequest/Create\" target=\"_blank\">Gửi yêu cầu báo giá</a>");
                    }
                }
                else {
                    //alert(news);
                    if (invisibleprice == 0) $("#chouponRoom_" + idroom).html("<span class=pricedetailunderline>" + formatCurrency(price) + " vnđ</span><a href=\"/HotelChoupon/Booking?id=" + id + "\" class=\"btn btn-sm btn-purple\" style=\"margin-left:4px;font-size:11px;\">" + formatCurrency(chouponprice) + " vnđ</a>");
                    else $("#chouponRoom_" + idroom).html("<a href=\"/HotelChoupon/Booking?id=" + id + "\" class=\"btn btn-sm btn-purple\" style=\"margin-left:4px;font-size:11px;\"> Coupon: " + formatCurrency(chouponprice) + " vnđ</a>");
                }
            
            }
        }
    }
    function getHotelListRoomDetail(idhotel, fromdate, todate, invisibleprice) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        formdata.append("fromdate", fromdate);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelRoom/getHotelRoomList');
        xhr.send(formdata);
        var content = "";

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";
                var hide = "";//"style=\"display:block;\"";
                //$("#listRoom").show();
                $("#roomList_" + idhotel).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var n = json_parsed.news[i];
                        var breakfast = "";
                        var extrabed = "";
                        var showprice = n.showprice;
                        //alert(name);
                        if (n.square) { breakfast += n.square+"m2";}
                        if (n.breakfast != null) { breakfast += ", " + n.breakfast; }
                        breakfast = "<font style='font-size:11px;'>" + breakfast + "</font>";
                        //if (n.extrabed == 1) { breakfast += ", có giường phụ"; }   
                        var buttonBooking = "/hotel/" + unicodeToNoMark(n.hotelname) + "-" + fromdate + "-" + todate + "-" + n.idhotel + "-" + n.id;
                        buttonBooking = "<a href=\"" + buttonBooking + "\" class=\"btn btn-info\" style=\"margin-top:4px;margin-bottom:4px;\">Đặt phòng</a>";
                        if (i == 4 && json_parsed.news.length>4) {
                            hide = "style=\"display:none;\"";
                            $("#roomList_" + idhotel).append("<tr class=td_price_room id=trroomList_" + idhotel + "_00><td colspan=3><a onclick=\"viewHotelListRoomDetail(" + idhotel + "," + json_parsed.news.length + ");\" style=\"cursor:pointer;\">xem thêm..</a></td></tr>");
                        }
                        $("#roomList_" + idhotel).append("<tr class=td_price_room " + hide + " id=trroomList_" + idhotel + "_" + i + "><td style=\"width:103px;\">" + buttonBooking + "</td><td style=\"padding-left:2px;width:60%;\"><a style=\"text-decoration:underline;cursor:pointer\" onclick=getRoomDetail(" + idhotel + "," + n.id + ");>" + n.roomname + "</a><br>" + breakfast + "</td><td style=\"padding-left:2px;width:30%;\" id=chouponRoom_" + n.id + "></td><tr>");
                        getHotelChouponDetail(idhotel, n.id, n.price, fromdate, todate, invisibleprice, showprice);
                    }
                }
                //alert(content);
                //$("#roomList_" + idhotel).html(content);
                //alert(news);
            }
        }
    }
    function viewHotelListRoomDetail(idhotel,l) {
        $("#trroomList_" + idhotel + "_00").hide();
        for (var ii = 4; ii < l; ii++) {
            $("#trroomList_" + idhotel+"_"+ ii).show();
        }
    }
    function getRoomDetail(idhotel,idroom) {
        var formdata = new FormData(); //FormData object
        formdata.append("idroom", idroom);
    
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelRoom/getRoomDetail');
        xhr.send(formdata);
        var content = "";
        showLoadingImage();
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";
                //$("#listRoom").show();
                $("#dvgetRoomDetail_" + idhotel).show();
                $("#dvgetRoomDetail_" + idhotel).html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var n = json_parsed.news[i];
                        var breakfast = "";
                        var extrabed = "";
                        //alert(name);
                        if (n.square) { breakfast += n.square + "m2"; }
                        if (n.breakfast != null) { breakfast += ", " + n.breakfast; }
                        if (n.maxofadult != null) { breakfast += ", số người tối đa: " + n.maxofadult; }
                        if (n.extrabed == 1) { breakfast += ", có giường phụ"; }
                        if (n.extrabedfee != 0) { breakfast += ", phụ thu giường phụ: " + n.extrabedfee; }
                        if (n.extraotherfee != 0) { breakfast += ", phụ thu lễ tết: " + n.extraotherfee; }
                        if (n.notefee != null) { breakfast += "<br>Ghi chú: "+n.notefee;}
                        $("#dvgetRoomDetail_" + idhotel).append("<b>Phòng: " + n.roomname + "</b><Br>" + breakfast);
                    }
                }
                hideLoadingImage();
                //alert(content);
                //$("#roomList_" + idhotel).html(content);
                //alert(news);
            }
        }
    }
    function getHotelListRoomBooking(idhotel, fromdate, invisibleprice,idroom) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        formdata.append("fromdate", fromdate);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelRoom/getHotelRoomList');
        xhr.send(formdata);
        var content = "";

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                var lengN=json_parsed.news.length;
                content = "";
                totalRoom = lengN;
                $("#HotelListRoom").html("");
                var found = false;
                for (var i = 0; i < lengN ; i++) {
                    if (json_parsed.news[i]) {

                        var n = json_parsed.news[i];
                        var breakfast = "";
                        var extrabed = "";
                        //alert(name);
                        //if (n.square) { breakfast += n.square+"m2"; }
                        //if (n.breakfast == 1) { breakfast += ", kèm ăn sáng"; }
                        //if (n.extrabed == 1) { breakfast += ", có giường phụ"; }   
                        var price = n.price;
                        if (discount > 0) {
                            price = price - discount * price / 100;
                        }
                        if (invisibleprice != 0)
                            price = "";
                        else
                            price = ", <b style=\"color:#00B08F;font-size:12px;\">" + formatCurrency(price) + " Vnđ/đêm</b>";
                        var style = "style=\"display:none;\"";
                        if (n.id == idroom) { style = "style=\"display:block;\""; found = true;}
                        content += "<div class=\"form-group\" id=dvdroom_"+i+" "+style+">";
                        content += "<label for=\"room\">" + n.roomname + price + "</label>";
                        content += "<select class=\"form-control\" name=\"noOfRoom_" + i + "\" id=\"noOfRoom_" + i + "\" idroom=\"" + n.id + "\" roomname=\"" + n.roomname + "\" onchange=\"selectRoomType(" + i + "," + lengN + ");\">";
                        content += "<option selected=\"selected\" value=0>Chọn số lượng</option>";
                        for (j = 1; j <= 6; j++) {
                            content +="<option value=\""+j+"\">"+j+"</option>";
                        }
                        content += "</select></div>"
                    }
                }
                //alert(content);
                $("#HotelListRoom").html(content);
                if (!found) { $("#dvdroom_0").show();}
                //alert(news);
            }
        }
    }
    function changeOtherRoomBooking() {
        for (var i = 0; i < 12; i++) {
            if (document.getElementById("dvdroom_" + i)) {
                $("#dvdroom_"+i).show();
            }
        }
    }
    function selectRoomType(index, Length) {
        if ($("#noOfRoom_" + index).val() > 0) {
            for (var j = 0; j < Length; j++) {
                if (j != index) $("#noOfRoom_" + j).val("0");
            }
        }
    }
    function getHotelListImagesBooking(idhotel) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelImages/getHotelListImagesBooking');
        xhr.send(formdata);
        var content = "";

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";

                $("#HotelSlider").html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var n = json_parsed.news[i];
                        var breakfast = "";
                        var extrabed = "";
                        //alert(name);
                        content += "<div class=\"item\"><a href=\""+n.name+"\" data-rel=\"prettyPhoto[gallery1]\"><img src=\""+n.name+"\" alt=\""+n.caption+"\" class=\"img-responsive\" width=\"750\" height=\"481\"></a> </div>";
                    
                    }
                }
                //alert(content);
                $("#HotelSlider").html(content);
                //alert(news);
            }
        }
    }
    function formatCurrency(number) {
        if (number == null || number == "") return 0;
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }
    function searchHotelSubmit() {
        if ($("#checkin").val() == "") {
            alert("Nhập ngày nhận phòng");
            $("#checkin").focus();
            return;
        }
        if ($("#checkout").val() == "") {
            alert("Nhập ngày trả phòng");
            $("#checkout").focus();
            return;
        }
        var name = $("#hotelname").val();
        name = removeSpecialCharater(name);
        //alert(name);
        var dis = $("#dis").val();
        var rate = $("#rate").val();
        var fdate = getDateId($("#checkin").val());
        var tdate = getDateId($("#checkout").val());
        //alert(name);
        //alert(dis);
        if (name == "" && dis == null) {
            alert("Bạn phải nhập ít nhất một trong hai: Tên khách sạn hoặc địa điểm");
            $("#dis").focus();
            return;
        }
        if (rate == null) rate = "0";
        if (dis == null) dis = "all";
        if (name.trim() == "") name = "all";
        name = name.trim();
        dis = dis.replace(/ /g, "_");
        name = name.replace(/ /g, "_");

        var url = "/";
        if (fdate != "") {
            url += fdate;
        }
        if (tdate != "") {
            url += "-" + tdate;
        }
        url += "-" + name;
        url += "-" + rate + "rate";
        url += "-" + dis;
        url += "-page1";
        window.location.href = url;
    }
    function searchHotelPromotionSubmit() {
        if ($("#checkin").val() == "") {
            alert("Nhập ngày nhận phòng");
            $("#checkin").focus();
            return;
        }
        if ($("#checkout").val() == "") {
            alert("Nhập ngày trả phòng");
            $("#checkout").focus();
            return;
        }
        var name = $("#hotelname").val();
        name = removeSpecialCharater(name);
        //alert(name);
        var dis = $("#dis").val();
        var rate = $("#rate").val();
        var fdate = getDateId($("#checkin").val());
        var tdate = getDateId($("#checkout").val());
        //alert(name);
        //alert(dis);
        //if (name == "" && dis == null) {
        //    alert("Bạn phải nhập ít nhất một trong hai: Tên khách sạn hoặc địa điểm");
        //    $("#dis").focus();
        //    return;
        //}
        if (rate == null) rate = "0";
        if (dis == null) dis = "all";
        if (name.trim() == "") name = "all";
        name = name.trim();
        dis = dis.replace(/ /g, "_");
        name = name.replace(/ /g, "_");

        var url = "/";
        if (fdate != "") {
            url += fdate;
        }
        if (tdate != "") {
            url += "-" + tdate;
        }
        url += "-" + name;
        url += "-" + rate + "rate";
        url += "-" + dis;
        url += "-page1";
        window.location.href = url;
    }
    function searchGolfSubmit(){
        var name = $("#golfname").val();
        window.location.href = "/Golf/List?name=" + name;
    }
    function getTotalPrice(idbooking,idhotel, idroom, numofroom, fromdate, todate, id,typebook) {
        var formdata = new FormData(); //FormData object
        formdata.append("idbooking", idbooking);
        formdata.append("idhotel", idhotel);
        formdata.append("idroom", idroom);
        formdata.append("numofroom", numofroom);
        formdata.append("fromdate", fromdate);
        formdata.append("todate", todate);
        formdata.append("typebook", typebook);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/HotelBooking/getTotalPrice');
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                $("#totalprice_" + id).html("Tổng tiền:<b>"+formatCurrency(xhr.responseText)+"</b>");
            }
        }
    }
    function getGolfTotalPrice(idbooking, idgolf, dateplay,typebook) {
        var formdata = new FormData(); //FormData object
        formdata.append("idbooking", idbooking);
        formdata.append("idgolf", idgolf);   
        formdata.append("dateplay", dateplay);   
        formdata.append("typebook", typebook);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/GolfBooking/getGolfTotalPrice');
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                $("#totalprice_" + idbooking).html("Tổng tiền:<b>" + formatCurrency(xhr.responseText) + "</b>");
            }
        }
    }
    function getCatNewsList(value) {
        //var formdata = new FormData(); //FormData object
        //formdata.append("keyword", keyword);
        //formdata.append("location", location);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/News/getCatNewsList');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#catname").html();
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {
                        var name = json_parsed.news[i].catname;
                        var id = json_parsed.news[i].id;
                        //alert(name);
                        $("#catname").append("<option value='" + name + "' catid=" + id + ">" + name + "</option>");
                    }
                }
                $("#catname").val(value);
                //alert(news);
            }
        }
    }
    function getImageShowContentList(url) {

        var formdata = new FormData(); //FormData object
        formdata.append("url", url);    
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/News/getImageShowContentList');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                $('#imageShowContent').append(xhr.responseText);
            }
        }
    }
    function getListSumCat() {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/News/getListSumCat');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";

                $("#listSumCat").html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var name = json_parsed.news[i].catname;
                        var catid = json_parsed.news[i].catid;
                        var total = json_parsed.news[i].total;
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        content += "<li><a href=\"/category/all-" + unicodeToNoMarkCat(name) + "-" + catid + "-1\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";

                    }
                }
                //alert(content);
                $("#listSumCat").html(content);
            }
        }
    }
    function getNewsGolfList() {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/News/getNewsGolfList');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";

                $("#listNewsGolf").html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var title = json_parsed.news[i].title;
                        var image = json_parsed.news[i].image;
                        var url = json_parsed.news[i].url;
                        var id = json_parsed.news[i].id;
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        content += "<tr><td><img src=\""+image+"\" width=200 height=100></td><td align=left style=\"padding-left:5px;\"><a href=\"/detail/"+url+"-"+id+"\"><b>"+title+"</b></td></tr>";

                    }
                }
                //alert(content);
                $("#listNewsGolf").html(content);
            }
        }
    }
    function getGolfListGallery() {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Golf/getGolfListGallery');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";

                $("#owl-gallery").html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var name = json_parsed.news[i].name;
                        var image = json_parsed.news[i].image;
                        var website = json_parsed.news[i].website;
                    
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        content += "<div class=\"item\"><a href=\""+website+"\" target=\"_blank\"><img src=\""+image+"\" width=\"800\" height=\"504\" alt=\""+name+"\"><i class=\"fa fa-search\"></i></a></div>";

                    }
                }
                //alert(content);
                $("#owl-gallery").html(content);
            }
        }
    }
    function checkTimeFormat(v) {    
        if (v.trim().length != 5) return false;
        var s1 = v.substring(0, 2);
        var s2 = v.substring(3, 5);    
        if (isNaN(s1) || isNaN(s2)) return false;
        //alert(parseInt(s1) + "-" + parseInt(s2));
        if (parseInt(s1) > 23 || parseInt(s2) > 60) return false;
        //alert(s1 + "-" + s2);
        if (parseInt(s1) < 1 || parseInt(s2) < 0) return false;
        //alert(s1 + "-" + s2);
        return true;
    }
    function selectChouponProvin() {
        window.location.href = "/HotelChoupon/List?provin=" + document.getElementById("provin").value;
    }
    function selectChouponGolfProvin() {
        window.location.href = "/GolfChoupon/List?provin=" + document.getElementById("provin").value;
    }
    function showLoadingImage() {
        $("#loadingImage").show();
    }
    function hideLoadingImage() {
        $("#loadingImage").hide();
    }
    function getGoodPrice(fromdate,todate) {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Hotels/getListProvince');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";

                $("#GoodPrice").html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var name = json_parsed.news[i];
                        var name2 = name.replace(/ /g, "_");
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        content += "<div class=\"col-md-3\"><a href=\"/GoodPrice/" + fromdate + "-" + todate + "-all-0rate-" + name2 + "-page1\" style=\"color:#2870B8;font-size:14px;\">Khách sạn " + name + " giá rẻ nhất</a></div>";

                    }
                }
                //alert(content);
                $("#GoodPrice").html(content);
            }
        }
    }
    function getTopDeal(fromdate, todate) {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/HotelPromotion/getTopDeal?fromdate=' + fromdate);
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";

                $("#TopDeal").html("<tr><th>Khách sạn</th><th>Giai đoạn áp dụng</th><th>Giá/đêm</th></tr>");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var name = json_parsed.news[i].hotelname;
                        var id = json_parsed.news[i].idhotel;
                        var fdate = convertFromDateIdToDateString(json_parsed.news[i].fdate.toString());
                        var tdate = convertFromDateIdToDateString(json_parsed.news[i].tdate.toString());
                        var price = json_parsed.news[i].price;
                        var discount = json_parsed.news[i].discount;
                        //alert(price + "-" + discount);
                        price=(price-price*discount/100);
                        var name2 = unicodeToNoMark(name);
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        //content += "<div class=\"col-md-3\"><a href=\"/hotel/"+name2+"-"+ fromdate + "-" + todate + "-" + id + "\" style=\"color:#64B342;font-size:14px;\">Khách sạn " + name + " giá rẻ nhất</a></div>";
                        content +="<tr>";
                        content +="	<td><a href=\"/hotel/"+name2+"-"+ fromdate + "-" + todate + "-" + id + "-0\"><h5>"+name+"</h5></a></td>";						
                        content +="	<td>"+fdate+"->"+tdate+"</td>";
                        content += "<td><b style=\"color:#00B08F;font-size:18px;\">" + formatCurrency(price) + "</b></td>";
                        content += "</tr>";
                    }
                }
                //alert(content);
                $("#TopDeal").html(content);
            }
        }
    }
    function getTopDealProvin(fromdate, todate,provin) {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/HotelPromotion/getTopDealByProvince?fromdate=' + fromdate+"&provin="+provin);
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";
                if (json_parsed.news.length <= 0) { $("#TopDealProvinDiv").hide(); return; }
                $("#TopDealProvinDiv").show();
                $("#TopDealProvin").html("<tr><th>Khách sạn</th><th>Giá/đêm</th></tr>");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var name = json_parsed.news[i].hotelname;
                        var id = json_parsed.news[i].idhotel;
                        var fdate = convertFromDateIdToDateString(json_parsed.news[i].fdate.toString());
                        var tdate = convertFromDateIdToDateString(json_parsed.news[i].tdate.toString());
                        var price = json_parsed.news[i].price;
                        var discount = json_parsed.news[i].discount;
                        //alert(price + "-" + discount);
                        price = (price - price * discount / 100);
                        var name2 = unicodeToNoMark(name);
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        //content += "<div class=\"col-md-3\"><a href=\"/hotel/"+name2+"-"+ fromdate + "-" + todate + "-" + id + "\" style=\"color:#64B342;font-size:14px;\">Khách sạn " + name + " giá rẻ nhất</a></div>";
                        content += "<tr>";
                        content += "	<td><a href=\"/hotel/" + name2 + "-" + fromdate + "-" + todate + "-" + id + "-0\"><h5>" + name + "</h5></a></td>";
                        //content += "	<td>" + fdate + "->" + tdate + "</td>";
                        content += "<td><b style=\"color:#00B08F;font-size:18px;\">" + formatCurrency(price) + "</b></td>";
                        content += "</tr>";
                    }
                }
                //alert(content);
                $("#TopDealProvin").html(content);
            }
        }
    }
    function getTopDealViews(fromdate, todate) {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/HotelPromotion/getTopDealViews');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "";
                if (json_parsed.news.length <= 0) { $("#TopDealViewsDiv").hide(); return; }
                $("#TopDealViewsDiv").show();
                $("#TopDealProvinView").html("<tr><th>Khách sạn</th><th>Tỉnh thành</th></tr>");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var name = json_parsed.news[i].name;
                        var id = json_parsed.news[i].id;
                        var provin = json_parsed.news[i].provin;
                        //var fdate = convertFromDateIdToDateString(json_parsed.news[i].fdate.toString());
                        //var tdate = convertFromDateIdToDateString(json_parsed.news[i].tdate.toString());
                        //var price = json_parsed.news[i].price;
                        //var discount = json_parsed.news[i].discount;
                        ////alert(price + "-" + discount);
                        //price = (price - price * discount / 100);
                        var name2 = unicodeToNoMark(name);
                        //alert(name);
                        //content += "<li><a href=\"/tin/" + unicodeToNoMark(name) + "\">" + name + "<span class=\"badge pull-right\">" + total + "</span></a></li>";
                        //content += "<div class=\"col-md-3\"><a href=\"/hotel/"+name2+"-"+ fromdate + "-" + todate + "-" + id + "\" style=\"color:#64B342;font-size:14px;\">Khách sạn " + name + " giá rẻ nhất</a></div>";
                        content += "<tr>";
                        content += "	<td><a href=\"/hotel/" + name2 + "-" + fromdate + "-" + todate + "-" + id + "-0\"><h5>" + name + "</h5></a></td>";
                        //content += "	<td>" + fdate + "->" + tdate + "</td>";
                        content += "<td><b style=\"color:#00B08F;font-size:14px;\">" + provin + "</b></td>";
                        content += "</tr>";
                    }
                }
                //alert(content);
                $("#TopDealProvinView").html(content);
            }
        }
    }
    function getFeedbackList(fromdate,todate) {

        //var formdata = new FormData(); //FormData object
        //formdata.append("url", url);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Feedback/getFeedbackList');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var news = '{"news":' + xhr.responseText + '}';
                //alert(news);
                var json_parsed = $.parseJSON(news);
                content = "<div class=\"item\">";
                //alert($("#TopFeedback").html());
                $("#owl-reviews").html("");
                for (var i = 0; i < json_parsed.news.length; i++) {
                    if (json_parsed.news[i]) {

                        var cname = json_parsed.news[i].cname;
                        var caddress = json_parsed.news[i].caddress;
                        var fullcontent = json_parsed.news[i].fullcontent;
                        var idhotel = json_parsed.news[i].idhotel;
                        var idgolf = json_parsed.news[i].idgolf;
                        var hotelimage = json_parsed.news[i].hotelimage;
                        var hotelname = json_parsed.news[i].hotelname;
                        var golfimage = json_parsed.news[i].golfimage;
                        var golfname = json_parsed.news[i].golfname;
                        var image=hotelimage!=null?hotelimage:golfimage;
                        var link="";
                        var name="";
                        var title="";
                        if (hotelimage!=null){
                            title=hotelname;
                            name = unicodeToNoMark(hotelname);
                            link="/hotel/"+name+"-"+ fromdate + "-" + todate + "-" + idhotel;
                        }else{
                            title=golfname;
                            link="/GolfBooking/Contact?idgolf="+idgolf;
                        }
                        content += "<div class=\"row\">";
                        content += " <div class=\"col-lg-3 col-md-4 col-sm-2 col-xs-12\"><a href=\"" + link + "\"><img src=\"" + image + "\" width=102 height=102 alt=\"" + title + "\" class=\"img-circle\"/></a></div>";
                        content +=" <div class=\"col-lg-9 col-md-8 col-sm-10 col-xs-12\">";
                        content +=" <div class=\"text-balloon\">"+fullcontent+"<span>"+cname+", "+caddress+"</span></div>";
                        content +="</div>";
                        content += "</div>";
                        //$("#TopFeedback").append("bbbbb");
                        if (i>=2 && i%2==0){
                            content +="</div><div class=\"item\">";
                        }
                    }
                }
                //alert(content);
                content+="</div>";
                $("#owl-reviews").html(content);
            }
        }
    }
    function updateTotalViews(idhotel) {
        var formdata = new FormData(); //FormData object
        formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Hotels/updateTotalViews');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
            }
        }
    }
    function getCompanyAddress() {
        //var formdata = new FormData(); //FormData object
        //formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/CompanyAddress/getCompanyAddress');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //alert(xhr.responseText);
                var news = '{"news":' + xhr.responseText + '}';            
                var json_parsed = $.parseJSON(news);
                $("#divCompanyAdress").html(json_parsed.news[0]);
                //alert(json_parsed.news[0]);
            }
        }
    }
    function getCompanyAddress2() {
        //var formdata = new FormData(); //FormData object
        //formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/CompanyAddress/getCompanyAddress');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //alert(xhr.responseText);
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#divCompanyAdress2").html(json_parsed.news[0]);
                //alert(json_parsed.news[0]);
            }
        }
    }
    function getCompanyIntro() {
        //var formdata = new FormData(); //FormData object
        //formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/CompanyIntro/getCompanyIntro');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //alert(xhr.responseText);
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#divCompanyIntro").html(json_parsed.news[0]);
                //alert(json_parsed.news[0]);
            }
        }
    }
    function getCompanyBanking() {
        //var formdata = new FormData(); //FormData object
        //formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/CompanyBanking/getCompanyBanking');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //alert(xhr.responseText);
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                $("#divCompanyBanking").html(json_parsed.news[0]);
                //alert(json_parsed.news[0]);
            }
        }
    }
    function getCompanyHotLine() {
        //var formdata = new FormData(); //FormData object
        //formdata.append("idhotel", idhotel);
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/HotLine/getCompanyHotLine');
        xhr.send();
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                //alert(xhr.responseText);
                var news = '{"news":' + xhr.responseText + '}';
                var json_parsed = $.parseJSON(news);
                var res = json_parsed.news[0].phone.split("/");
                var resemail = json_parsed.news[0].email.split("/");
                var thephone = "";
                var theemail = "";
                //for (var l = 0; l < res.length; l++) {
                //    thephone += "<p style=\"font-weight:bold;color:#363636;font-size:18px;margin-top:2px;\">" + res[l] + "</p>";
                //}
                for (var l = 0; l < resemail.length; l++) {
                    theemail += "<li><a href=\"mailto:" + resemail[l] + "\" style=\"color:#ffffff;\">" + resemail[l] + "</a></li>";
                }
                var content = "<div ><a href=\"#\" class=\"hotlinetext\"><i class=\"fa fa-phone\"></i><font style=\"margin-top:1px;\" class=\"hotlinetext\">Hotline:</font> <B style=\"margin-top:1px;\" class=\"hotlinephone\">" + res[0] + "</b></a> </div>";
                if (res.length >= 2) content += "<div  style=\"margin-top:8px;\"><B class=\"hotlinephone2\">" + res[1] + "</b></div>";
                //content += "<div class=\"th-item\"><a href=\"#\" style='color:#fff'><i class=\"fa fa-envelope\"></i>" + json_parsed.news[0].email + "</a></div>";
                $("#divCompanyHotLine").html(content);
                $("#ulEmailContact").html(theemail);
                //alert(json_parsed.news[0]);
            }
        }
    }
    function addnewsletter() {
        var formdata = new FormData(); //FormData object
        var cemail=document.getElementById("newsletter").value;
        formdata.append("cemail", cemail);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/CustomerEmail/newsletter');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                alert("Cập nhật email thành công! Chúng tôi sẽ liên hệ với bạn khi có các chương trình mới!");
            }
        }
    }
    function Login() {
        var formdata = new FormData(); //FormData object
        var name = document.getElementById("name").value;
        var pass = document.getElementById("pass").value;
        formdata.append("name", name);
        formdata.append("pass", pass);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/User/Login');
        xhr.send(formdata);
        var content = "";
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (xhr.responseText == 0) {
                    alert("Sai user hoặc mật khẩu!");
                } else {
                    window.location.href = "/Admin/Index";
                }
            }
        }
    }
    function isEmail(email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        ///^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$./;
        return re.test(email);
    }