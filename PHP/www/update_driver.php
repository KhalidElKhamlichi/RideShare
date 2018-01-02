<?php  
   $response = array(); 
   if (isset($_POST['img_name']) && isset($_POST['license_plate']) && isset($_POST['car_type']) && isset($_POST['car_color']) && isset($_POST['username'])) {
      $img_name = $_POST['img_name'];  
      $license_plate = $_POST['license_plate'];  
      $car_type = $_POST['car_type'];
      $car_color = $_POST['car_color']; 
      $username = $_POST['username']; 
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      $stmt = $conn->prepare("INSERT INTO cars(LicensePlate, Type, Color) VALUES(?, ?, ?)");
      $stmt->bind_param("sss", $license_plate, $car_type, $car_color); 
      $stmt->execute(); 
      // check if row inserted or not  
      if ($stmt) 
      {  
         $response["success_msg"] = 1;  
         $response["message"] = "Successfully registered.";  
      } 
      else 
      {  
         // failed to insert row  
         $response["success_msg "] = 0;  
         $response["message"] = "Error occured, not registered.";  
      }  
      
      //The image is encoded in base 64 encoding, decode it
      //$imgData = base64_decode($license_img);
      
      //Update user with image
      
      $stmt2 = $conn->prepare("UPDATE users SET LicenseImg =? WHERE Username=?");
      $stmt2->bind_param("ss", $img_name, $username); 
      $stmt2->execute(); 
   

      $stmt3 = $conn->prepare("SELECT Id FROM cars WHERE LicensePlate=?");
      $stmt3->bind_param("s", $license_plate); 
      $stmt3->execute();
      $res = $stmt3->get_result();
      $row = $res->fetch_assoc();
      $Id = $row['Id'];   

      $stmt4 = $conn->prepare("UPDATE users SET Car = ? WHERE Username=?");
      $stmt4->bind_param("ss", $Id, $username); 
      $stmt4->execute(); 

      echo $response["message"];
      
   }


?>  