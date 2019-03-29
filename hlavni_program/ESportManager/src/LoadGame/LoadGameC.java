/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package LoadGame;

import InGameMenu.InGameMenuClass;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.Node;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.control.TextField;
import javafx.scene.input.MouseEvent;
import javafx.stage.Stage;
import javafx.stage.Window;

/**
 *
 * @author isek
 */
public class LoadGameC implements Initializable {
    
    
    @FXML
    private ListView<?> listView;
    @FXML
    private Button load;
    @FXML
    private Button back;
    Stage stage = new Stage();

    @Override
    public void initialize(URL url, ResourceBundle rb) {
        //přidat uložené hry do listview
        
    }    

    

    @FXML
    private void loadClick(MouseEvent event) throws Exception {
       // Button but = (Button) event.getSource();
        Node  source = (Node)  event.getSource(); 
    Stage stage  = (Stage) source.getScene().getWindow();
    
    stage.close();
        
        new InGameMenuClass(stage);
        stage.setAlwaysOnTop(true);
        //disableButtons();
    }

    @FXML
    private void backClick(MouseEvent event) {
        Stage stage = (Stage) back.getScene().getWindow();
        stage.close();
    }
    
}
