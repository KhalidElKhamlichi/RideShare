<?php
$uploads_dir = 'UsersImgs/'; 
if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) 
 {
     $tmp_name = $_FILES["file"]["tmp_name"];
     $name = $_FILES["file"]["name"];
     $link= $uploads_dir."/".$name;
     if( move_uploaded_file($tmp_name, $link))
     {
        echo "Success: Picture Upload Successfully!";
     }
     else
     {
        echo "There was an error uploading the file, please try again!".$link;
     }
  }
  else
  {
     echo "Error: Picture not Uploaded";
  }
?>