<?php  
   
   if (isset($_POST['origin']) && isset($_POST['destination']) && isset($_POST['date']) && isset($_POST['time']) && isset($_POST['stops']) && isset($_POST['smokes']) && isset($_POST['backbag']) && isset($_POST['travelbag'])) 
   {
      $origin = $_POST['origin'];  
      $destination = $_POST['destination'];  
      $date = $_POST['date'];
      $time = $_POST['time'];
      $stops = ($_POST['stops'] === 'True');
      $stops =  ($stops?1:0);
      $smokes = ($_POST['smokes'] === 'True');
      $smokes = ($smokes?1:0);
      $backbag = $_POST['backbag'];
      $travelbag = $_POST['travelbag'];

      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();

      
      
      if(isset($_POST['seats']) && isset($_POST['driver']))
      {
         $seats = $_POST['seats'];
         $seats = intval($seats);
         $driver = $_POST['driver'];
         
         $userStmt = $conn->prepare("SELECT id FROM users WHERE Username=?");
         $userStmt->bind_param("s", $driver); 
         $userStmt->execute();
         $res = $userStmt->get_result();
         $row = $res->fetch_assoc(); 
         $id = $row["id"];

         
         $stmt = $conn->prepare("INSERT INTO rides(Origin, Destination, RideDate, StartTime, Stops, Smokes, Seats, Backbag, TravelBag, Driver) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
         $stmt->bind_param("ssssiiissi", $origin, $destination, $date, $time, $stops, $smokes, $seats, $backbag, $travelbag, $id); 
         $stmt->execute(); 
         // check if row inserted or not  
         if ($stmt) 
         {  
            $response = "Success.";  
         } 
         else 
         {  
            // failed to insert row  
            $response = "Error occured.";  
         }  
         
         
      }
      else
      {
         if(isset($_POST['submitter']))
         {
            $submitter = $_POST['submitter'];

            $userStmt = $conn->prepare("SELECT id FROM users WHERE Username=?");
            $userStmt->bind_param("s", $submitter); 
            $userStmt->execute();
            $res = $userStmt->get_result();
            $row = $res->fetch_assoc(); 
            $id = $row["id"];

            $stmt = $conn->prepare("INSERT INTO rides(Origin, Destination, RideDate, StartTime, Stops, Smokes, Backbag, TravelBag, Submitter) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?)");
            $stmt->bind_param("ssssiissi", $origin, $destination, $date, $time, $stops, $smokes, $backbag, $travelbag, $id); 
            $stmt->execute(); 
            // check if row inserted or not  
            if ($stmt) 
            {  
               $response = "Success. Demand submitted.";  
            } 
            else 
            {  
               // failed to insert row  
               $response = "Error occured.";  
            }  
         }
            
      }
         
      echo $response;
      
   }


?>  