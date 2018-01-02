<?php  
   if (isset($_POST['username']) && isset($_POST['ride_id'])) {
      $username = $_POST['username']; 
      $ride_id = $_POST['ride_id'];
      $ride_id = intval($ride_id);
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      $checkStmt = $conn->prepare("SELECT * FROM users WHERE Username=?");
      $checkStmt->bind_param("s", $username); 
      $checkStmt->execute();
      $res = $checkStmt->get_result();
      $row = $res->fetch_assoc();
      $user_id = $row['id'];

      //check if user already joined ride
      $checkStmt2 = $conn->prepare("SELECT * FROM users2rides WHERE UserId=? AND RideId=?");
      $checkStmt2->bind_param("ii", $user_id, $ride_id); 
      $checkStmt2->execute();
      $res2 = $checkStmt2->get_result();
      
      if($res2->fetch_assoc() == NULL)
      {

         $seats_stmt = $conn->prepare("SELECT * FROM rides WHERE Driver=?");
         $seats_stmt->bind_param("i", $user_id); 
         $seats_stmt->execute();
         $ride_res = $seats_stmt->get_result();
         $ride_row = $ride_res->fetch_assoc();
         $ride_seats = $ride_row['Seats'];
         $seats = intval($ride_seats);
         
         if($seats > 0)
         {
            $stmt = $conn->prepare("INSERT INTO users2rides(UserId, RideId) VALUES(?, ?)");
            $stmt->bind_param("ii", $user_id, $ride_id); 
            $stmt->execute();
            // check if row inserted or not  
            if ($stmt) 
            {  
               $new_seats = $seats - 1;
               $up_stmt = $conn->prepare("UPDATE rides SET Seats=? WHERE Id=?");
               $up_stmt->bind_param("ii", $new_seats, $ride_id); 
               $up_stmt->execute(); 
               $response = "Successfully joined.";  
            } 
            else 
            {  
               
               $response = "Error occured, not joined.";  
            }  
         }
         else
         {
            $response = "ride full.";
         }
   
      }
      else
      {
         $response = "already joined.";
      }

      echo $response;
      
   }


?>  