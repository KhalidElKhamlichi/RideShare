<?php  
   if (isset($_POST['ride_id'])) 
   {

      $ride_id = $_POST['ride_id']; 
      $r_id = intval($ride_id);
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      $delStmt = $conn->prepare("DELETE FROM rides WHERE Id=?");
      $delStmt->bind_param("i", $r_id); 
      $delStmt->execute();
		  
	}

?>  

