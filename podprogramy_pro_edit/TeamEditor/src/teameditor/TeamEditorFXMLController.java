/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package teameditor;

import java.io.File;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.control.MenuItem;
import javafx.scene.control.TextField;
import javafx.stage.FileChooser;

/**
 *
 * @author Gunny
 */
public class TeamEditorFXMLController implements Initializable {
    
    private Label label;
    private Button add;
    @FXML
    private ComboBox<String> narodnostCB;
    @FXML
    private TextField nazevTymuTF;
    @FXML
    private ListView<String> seznamTymuLB;
    @FXML
    private Button editTymB;
    @FXML
    private Button smazTymB;
    @FXML
    private Button PridatNovyTymB;
    
    private void handleButtonAction(ActionEvent event) {
        System.out.println("You clicked me!");
        label.setText("Hello World!");
    }
    
    @Override
    public void initialize(URL url, ResourceBundle rb) {
        SQLiteJDBC f = new SQLiteJDBC();
    }    

    private void nacistTym(ActionEvent event) {
        
    }

    @FXML
    private void editujTym(ActionEvent event) {
    }

    @FXML
    private void smazatTym(ActionEvent event) {
    }

    @FXML
    private void pridatTym(ActionEvent event) {
    }

    
}
