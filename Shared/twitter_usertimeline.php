<?php
header('Content-Type:text/plain');

$url						= "https://api.twitter.com/1.1/statuses/user_timeline.json";
$oauth_access_token			= "INSERT_HERE";
$oauth_access_token_secret	= "INSERT_HERE";
$consumer_key				= "INSERT_HERE";
$consumer_secret			= "INSERT_HERE";
$use_curl					= false;
$cache_file                 = dirname(__FILE__) . '/twitter_usertimeline.cache.json';
$cache_interval             = 60;

function buildBaseString($baseURI, $method, $params) {
    $r = array();
    ksort($params);
    foreach($params as $key=>$value){
        $r[] = "$key=" . rawurlencode($value);
    }
    return $method."&" . rawurlencode($baseURI) . '&' . rawurlencode(implode('&', $r));
}

function buildAuthorizationHeader($oauth) {
    $r = 'Authorization: OAuth ';
    $values = array();
    foreach($oauth as $key=>$value)
        $values[] = "$key=\"" . rawurlencode($value) . "\"";
    $r .= implode(', ', $values);
    return $r;
}

//Generating OAuth request data 
$oauth = array( 'oauth_consumer_key' => $consumer_key,
                'oauth_nonce' => time(),
                'oauth_signature_method' => 'HMAC-SHA1',
                'oauth_timestamp' => time(),
                'oauth_token' => $oauth_access_token,
                'oauth_version' => '1.0');
				
$base_info = buildBaseString($url, 'GET', $oauth);
$composite_key = rawurlencode($consumer_secret) . '&' . rawurlencode($oauth_access_token_secret);
$oauth_signature = base64_encode(hash_hmac('sha1', $base_info, $composite_key, true));
$oauth['oauth_signature'] = $oauth_signature;

$output = '';
if (file_exists($cache_file) && (filemtime($cache_file) > (time() - $cache_interval))) {
	// Cache file is less than five minutes old. 
	// Don't bother refreshing, just use the file as-is.
	$output = file_get_contents($cache_file);
} else {
	// Our cache is out-of-date, so load the data from our remote server,
	// and also save it over our cache for next time.
	//Make request
	if ($use_curl) {
		$header = array(buildAuthorizationHeader($oauth), 'Expect:');
		$options = array( CURLOPT_HTTPHEADER => $header,
						  //CURLOPT_POSTFIELDS => $postfields,
						  CURLOPT_HEADER => false,
						  CURLOPT_URL => $url,
						  CURLOPT_RETURNTRANSFER => true,
						  CURLOPT_SSL_VERIFYPEER => false);

		$feed = curl_init();
		curl_setopt_array($feed, $options);
		$output = curl_exec($feed);
		curl_close($feed);
		file_put_contents($cache_file, $output, LOCK_EX);
	} else {
		$fp = fsockopen("ssl://api.twitter.com", 443, $errno, $errstr, 10);
		if (!$fp) {
			echo "$errstr ($errno)<br />\n";
		} else {
			$out = "GET /1.1/statuses/user_timeline.json HTTP/1.1\r\n"
			."Host: api.twitter.com\r\n"
			.buildAuthorizationHeader($oauth)."\r\n"
			."Content-type: application/x-www-form-urlencoded\r\n"
			."Connection: Close\r\n\r\n";
			fwrite($fp, $out);
			
			$header = '';
			while ( strpos ( $header, "\r\n\r\n" ) === false )
				$header .= fgets($fp, 128);
			
			while (!feof($fp)) {
				$output = $output . fgets($fp, 128);
			}
			fclose($fp);
			file_put_contents($cache_file, $output, LOCK_EX);
		}
	}
}

echo $output;
?>