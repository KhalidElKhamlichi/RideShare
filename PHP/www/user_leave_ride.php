<?php  
   if (isset($_POST['ride_id']) && isset($_POST['username'])) 
   {

      $ride_id = $_POST['ride_id']; 
      $r_id = intval($ride_id);
      $username = $_POST['username'];
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();

      $checkStmt = $conn->prepare("SELECT Id FROM users WHERE Username=?");
      $checkStmt->bind_param("s", $username); 
      $checkStmt->execute();
      $res = $checkStmt->get_result();
      $row = $res->fetch_assoc();
      $u_id = intval($row['Id']);
      
      $delStmt = $conn->prepare("DELETE FROM users2rides WHERE UserId=? AND RideId=?");
      $delStmt->bind_param("ii", $u_id, $r_id); 
      $delStmt->execute();

      $stmt = $conn->prepare("UPDATE rides SET Seats=Seats+1 WHERE Id=?");
      $stmt->bind_param("i", $r_id); 
      $stmt->execute(); 
		  
	}

?>  

