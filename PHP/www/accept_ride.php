<?php  
   if (isset($_POST['seats']) && isset($_POST['username']) && isset($_POST['ride_id'])) {
      
      $username = $_POST['username']; 
      $seats = $_POST['seats'];
      $s = intval($seats); 
      $ride_id = $_POST['ride_id']; 
      $r = intval($ride_id); 
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();

      $userStmt = $conn->prepare("SELECT id FROM users WHERE Username=?");
      $userStmt->bind_param("s", $username); 
      $userStmt->execute();
      $res = $userStmt->get_result();
      $row = $res->fetch_assoc(); 
      $driver_id = $row["id"];
   
      //Update ride
      $stmt = $conn->prepare("UPDATE rides SET Seats=?, Driver=? WHERE Id=?");
      $stmt->bind_param("iii", $s, $driver_id, $r); 
      $stmt->execute(); 
      // check if row inserted or not  
      if ($stmt) 
      {  
         $response = "Successfully accepted.";  
      } 
      else 
      {  
         // failed to insert row  
         $response = "Error occured.";  
      }  
      
      
      echo $response;
      
   }


?>  