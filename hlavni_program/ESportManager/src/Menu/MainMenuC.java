/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Menu;

//import LoadGame.InGameMenuClass;
import Help.HelpClass;
import LoadGame.LoadGameClass;
import NewGame.NewGameClass;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.application.Platform;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.Background;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

/**
 *
 * @author isek
 */
public class MainMenuC implements Initializable {
    
    @FXML
    private Button newGame;
    @FXML
    private Button loadGame;
    @FXML
    private Button help;
    @FXML
    private Button exit;
    Stage stage = new Stage();
    @FXML
    private VBox vbox;
    
    
    @Override
    public void initialize(URL url, ResourceBundle rb) {
        
        
    }    

    @FXML
    private void newClick(MouseEvent event) throws Exception {
        new NewGameClass(stage);
        stage.setAlwaysOnTop(true);
        disableButtons();
    }

    @FXML
    private void loadClick(MouseEvent event) throws Exception {
        new LoadGameClass(stage);
        stage.setAlwaysOnTop(true);
        disableButtons();
    }

    @FXML
    private void helpClick(MouseEvent event) throws Exception {
        new HelpClass(stage);
        stage.setAlwaysOnTop(true);
        disableButtons();
    }

    @FXML
    private void exitClick(MouseEvent event) {
        Platform.exit();
    }
    
    private void disableButtons(){
       newGame.disableProperty().set(true);
    loadGame.disableProperty().set(true);
    help.disableProperty().set(true);
    exit.disableProperty().set(true);
    }

    private void enableButtons() {
        newGame.disableProperty().set(false);
    loadGame.disableProperty().set(false);
    help.disableProperty().set(false);
    exit.disableProperty().set(false);
    }
    
    private void active(){
        if (!stage.isShowing()){
            enableButtons();
        }
    }

    @FXML
    private void move(MouseEvent event) {
        active();
    }
}
