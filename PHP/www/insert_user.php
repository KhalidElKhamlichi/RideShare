<?php  
   $response = array(); 
   if (isset($_POST['fullname']) && isset($_POST['email']) && isset($_POST['phonenumber']) && isset($_POST['username']) && isset($_POST['password']) && isset($_POST['type'])) {
      $fullname = $_POST['fullname'];  
      $email = $_POST['email'];  
      $phonenumber = $_POST['phonenumber'];
      $username = $_POST['username']; 
      $password = $_POST['password'];
      $type = $_POST['type'];
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

      //check if username already exists
      if($row["Username"] == '')
      {
         $stmt = $conn->prepare("INSERT INTO users(Fullname, Email, Phonenumber, Username, Password, Type) VALUES(?, ?, ?, ?, ?, ?)");
         $stmt->bind_param("ssssss", $fullname, $email, $phonenumber, $username, $password, $type); 
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
      }
      else
      {
            $response["message"] = "Username already exists.";  
      }
      echo $response["message"];
      echo $row["Username"];
      
   }


?>  