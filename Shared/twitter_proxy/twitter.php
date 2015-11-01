<?php
session_start();
require_once("twitteroauth/twitteroauth.php");

function getConnectionWithAccessToken($cons_key, $cons_secret, $oauth_token, $oauth_token_secret) {
  $connection = new TwitterOAuth($cons_key, $cons_secret, $oauth_token, $oauth_token_secret);
  return $connection;
}

function getUserName() {
	$twitteruser = "";
	if(isset($_GET["user"]) && preg_match('/^[A-Za-z0-9_]{1,15}$/', $_GET["user"])) {
	   $twitteruser = trim($_GET["user"]);
	} else{
	   $twitteruser = "DMOWiki";
	}
	return $twitteruser;
}

$notweets					= 30;
$cache_interval             = 60;
$consumerkey				= "INSERT_HERE";
$consumersecret				= "INSERT_HERE";
$accesstoken				= "INSERT_HERE";
$accesstokensecret			= "INSERT_HERE";
$twitteruser 				= getUserName();
$cache_file                 = dirname(__FILE__) . '/twitter_cache/'.$twitteruser.'.json';
$output = '';
if (file_exists($cache_file) && (filemtime($cache_file) > (time() - $cache_interval))) {
	$output = file_get_contents($cache_file);
} else {
	$connection = getConnectionWithAccessToken($consumerkey, $consumersecret, $accesstoken, $accesstokensecret);
	$tweets = $connection->get("https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=".$twitteruser."&count=".$notweets);
	$output = json_encode($tweets);
	file_put_contents($cache_file, $output, LOCK_EX);
}

header('Content-Type: application/json');
echo $output;
?>
