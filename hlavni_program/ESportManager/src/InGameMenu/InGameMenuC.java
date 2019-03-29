/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package InGameMenu;

import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.control.TextField;
import javafx.scene.input.MouseEvent;
import javafx.stage.Stage;

/**
 *
 * @author isek
 */
public class InGameMenuC implements Initializable {
    
    
    private Button back;
    
    @Override
    public void initialize(URL url, ResourceBundle rb) {
       
    }    

    


    private void backClick(MouseEvent event) {
        Stage stage = (Stage) back.getScene().getWindow();
        stage.close();
    }
    
}
