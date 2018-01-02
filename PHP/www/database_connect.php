

<?php  
	define('DB_USER', "root"); // db user  
	define('DB_PASSWORD', ""); // db password  
	define('DB_DATABASE', "ridesharedb"); // database name  
	define('DB_SERVER', "localhost"); // db server/ host name 
	class DATABASE_CONNECT { 
		public $CONN; 
		// constructor  
		function __construct() {  
			// connecting to database  
			//$this->connect();  
		}  
		// destructor  
		function __destruct() {  
			// closing db connection  
			$this->close();  
		}  
		function connect() {  
			$con = mysqli_connect(DB_SERVER, DB_USER, DB_PASSWORD) or die(mysqli_error());
			$this->CONN = $con;  
			$db = mysqli_select_db($con, DB_DATABASE) or die(mysqli_error()) or die(mysqli_error());  
			return $con;  
		}  
		/** 
		* Function to close db connection 
		*/  
		function close() {  
			// closing db connection  
			mysqli_close($this->CONN);  
		}  
	} 
	 
?>  
