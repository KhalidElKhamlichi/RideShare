<?php  
   if (isset($_POST['img_name']) && isset($_POST['username']) && isset($_POST['newusername']) && isset($_POST['phonenumber']) && isset($_POST['email']) && isset($_POST['birthdate'])) {
      $img_name = $_POST['img_name'];  
      $phonenumber = $_POST['phonenumber'];
      $email = $_POST['email'];
      $birthdate = $_POST['birthdate'];
      $username = $_POST['username']; 
      $newusername = $_POST['newusername']; 
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      //Update user    
      $stmt = $conn->prepare("UPDATE users SET ImgPath=?, Phonenumber=?, BirthDate=?, Email=?, Username=? WHERE Username=?");
      $stmt->bind_param("ssssss", $img_name, $phonenumber, $birthdate, $email, $newusername, $username); 
      $stmt->execute(); 
      // check if row inserted or not  
      if ($stmt) 
      {  
         $response = "Successfully Saved.";  
      } 
      else 
      {  
         // failed to insert row  
         $response = "Error occured, not saved.";  
      }  
      
      
      echo $response;
      
   }


?>  