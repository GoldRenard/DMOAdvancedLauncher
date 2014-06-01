function humanFileSize(bytes, si) {
    var thresh = si ? 1000 : 1024;
    if(bytes < thresh) return bytes + ' B';
    var units = si ? ['kB','MB','GB','TB','PB','EB','ZB','YB'] : ['KiB','MiB','GiB','TiB','PiB','EiB','ZiB','YiB'];
    var u = -1;
    do {
        bytes /= thresh;
        ++u;
    } while(bytes >= thresh);
    return bytes.toFixed(1)+' '+units[u];
}

function humanDate(date) {
	var d = new Date();
	d.setTime(Date.parse(date));
	return d.toLocaleString();
}

function jsonLoad(data) {
	var release = data[0];
	var asset = release["assets"][0];
	var author = release["author"];
	if (asset != null && author != null) {
		
		var download_url = "https://github.com/GoldRenard/DMOAdvancedLauncher/releases/download/" + release["tag_name"] + "/" + asset["name"];
		
		$("#release_name").html("<a href=\"" + download_url + "\">" + release["name"] + "</a>");
		$("#release_pubdate").text(humanDate(release["published_at"]));
		$("#release_size").text(humanFileSize(asset["size"], 1024));
		$("#release_dls").text(asset["download_count"]);
		
		$("#release_author_link").attr('href', author["html_url"]);
		$("#release_author_avatar").attr('src', author["avatar_url"]);
		$("#release_author_name").text(author["login"]);
		
		$("#release_stats").css('display', 'block');
	}
}

$(document).ready(function() {
	$.getJSON("https://api.github.com/repos/GoldRenard/DMOAdvancedLauncher/releases", jsonLoad);
});