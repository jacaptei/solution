
Number.prototype.Pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
}

String.prototype.ReplaceAll || (function () {
    var regMetaChars = /[-\\^$*+?.()|[\]{}]/g;
    String.prototype.replaceAll = function (needle, replacement) {
        return this.replace(new RegExp(needle.replace(regMetaChars, '\\$&'), 'g'), replacement);
    };
}());

Number.prototype.FormatMoneyCustom = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

Number.prototype.FormatMoney = function () {
    var n = this,
        c = 2,
        d = ",",
        t = ".",
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return "R$"+s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

Array.prototype.ToList = function () {
    if (this.length == 0)
        return "";
    var html = "";
    this.forEach((item) => { html += "<div style='margin-top:4px'>&bull; " + item + "</div>" });
    return html;
}

Array.prototype.ToHtmlList = function () {
    if (this.length == 0)
        return "";
    //this.forEach((item) => { html += "<tr style='vertical-align:top'><td>&bull; &nbsp;</td><td>" + item + "</td></tr>" });
    //html += "</table>";
    var html = "";
    this.forEach((item) => { html += "<div style='margin-top:4px'>&bull; " + item + "</div>" });
    return html;
}
