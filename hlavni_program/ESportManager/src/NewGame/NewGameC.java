/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package NewGame;

import Menu.ESportManager;
import Menu.MainMenuC;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.scene.input.MouseEvent;
import javafx.stage.Stage;

/**
 *
 * @author isek
 */
public class NewGameC implements Initializable {
    
    @FXML
    private TextField gameName;
    @FXML
    private TextField firstName;
    @FXML
    private TextField lastName;
    @FXML
    private ComboBox<String> nationality;
    @FXML
    private ComboBox<String> league;
    @FXML
    private ComboBox<String> team;
    @FXML
    private Button start;
    
    @Override
    public void initialize(URL url, ResourceBundle rb) {
        nationality.getItems().add("AAA");
        league.getItems().add("AAA");
        team.getItems().add("AAA");
    }    

    @FXML
    private void startClick(MouseEvent event) {
        // přidat všechny atributy do databáze s hrama
        
        Stage stage = (Stage) start.getScene().getWindow();
        stage.close();
    }

    @FXML
    private void move(MouseEvent event) {
        if (league.getValue()!=null){
            team.disableProperty().set(false);
        }
        
        if (league.getValue()!=null && team.getValue()!=null && nationality.getValue()!=null && gameName.getText()!="" && firstName.getText()!="" && lastName.getText()!=""){
            start.disableProperty().set(false);
        }
    }
    
}
