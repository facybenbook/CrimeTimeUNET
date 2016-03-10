/**
 *	Keeps track of UI information
 *	UPDATE AT THE VERY BEGINNING OF CODE SO SCREEN INFO
 *	IS ACCURATE
 *	
 * 	TODO: should also have preloader info?? maybe???
 */
var UI = {
	width: 0,
	height: 0
}

/**
 * 	Titlecard code
 * 	Constructor: Generates the base of the titlecard
 *	TODO
 */
var Screen = function(){
	var screen_bg = new createjs.Shape();
	var screen_fg = new createjs.Shape();
	screen_bg.graphics.beginFill("#000").drawRect(0, 0, UI.width, UI.height);
	screen_fg.graphics.beginFill("DimGray").drawRoundRect(
		UI.width/16, UI.height/16, 
		UI.width - UI.width/8, UI.height - UI.height/8, 5);
	var borderimg = preloader.preload.getResult("startupBorderImg");
	borders = new createjs.Container();
	var borderBitmap = new createjs.Bitmap(borderimg);
	var border_offset = 20;
	for(i = 1; i <= 4; i++){
		var border = borderBitmap.clone();
		border.rotation = (90*i)%360;
		var popupwidth = UI.width - UI.width/8;
		var popupheight = UI.height - UI.height/8;
		var bounds = border.getBounds();
		switch(i){
			//top-left (0,0)
			case 1: //top-right
				border.x = popupwidth - border_offset;
				break;
			case 3: //bottom-left
				border.y = popupheight - border_offset;
				break;
			case 2: //bottom-right
				border.x = popupwidth - border_offset;
				border.y = popupheight - border_offset;
				break;
		}
		border.scaleX = border.scaleY = mobile_offset;
		borders.addChild(border);
	}
	borders.x = w/16 + border_offset/2;
	borders.y = h/16 + border_offset/2;
	this.addChild(screen_bg, screen_fg);
	return screen;
};

/**
 * 	Adds title to titlecard
 */
Screen.prototype.addTitle = function(text_settings) {
	//TODO
};

/**
 *	Adds description to titlecard
 */ 
Screen.prototype.addDesc = function(text_settings) {
	//TODO
};

/**
 * Adds brackets to title card
 */
Screen.prototype.addBrackets = function() {
	//TODO
};

_extends(Screen, createjs.Container);

/**
 * box_settings {w, h, r, color}
 * text_settings {text, size, font, color}
 */
var Button = function(box_settings, text_settings){
	this._super.constructor.call(this);

	//If certain settings don't exist, then set defaults
	box_settings = box_settings || {};
	text_settings = text_settings || {};

	box_settings['width'] = box_settings.width || 100;
	box_settings['height'] = box_settings.height || 100;
	box_settings['radius'] = box_settings.radius || 5;
	box_settings['color'] = box_settings.color || "Grey";

	text_settings['text'] = text_settings.text || "";
	text_settings['size'] = text_settings.size || 10;
	text_settings['font'] = text_settings.font || "DAYPBL";
	text_settings['color'] = text_settings.color || "white";

	this.box_settings = box_settings;
	this.text_settings = text_settings;

	var button_box = new createjs.Shape();
	var button_text = new createjs.Text(
		text_settings.text, 
		text_settings.size + "px " + text_settings.font,
		text_settings.color);
	button_box.graphics.beginFill(box_settings.color).drawRoundRect(
		0, 0, 
		box_settings.width, 
		box_settings.height, 
		box_settings.radius);

	button_text.x = box_settings.width/2 - button_text.getBounds().width/2;
	button_text.y = box_settings.height/2 - button_text.getBounds().height/2;

	this.button_box = button_box;
	this.button_text = button_text;

	this.addChild(button_box, button_text);
	return this;
};

Button.prototype.highlight = function(new_box_color, new_text_color) {
	this.box_settings['color'] = new_box_color || this.box_settings.color;
	this.text_settings['color'] = new_text_color || this.text_settings.color;

	var box_settings = this.box_settings;
	var text_settings = this.text_settings;

	this.button_box.graphics.clear().beginFill(new_box_color).drawRoundRect(0, 0, 
		box_settings.width, 
		box_settings.height, 
		box_settings.radius);
	this.button_text.color = new_text_color;
}

_extends(Button, createjs.Container);

var Stats = function(player) {
	this._super.constructor.call(this);

	this.max_stat = 10;

	this.name = new createjs.Text(player.username, "bold 30px Cinzel", "Black");
	this.hp = new PipBar(player.character.stats.hp);
	this.gun = new PipBar(player.character.stats.gun);
	this.melee = new PipBar(player.character.stats.melee);
	this.defense = new PipBar(player.character.stats.defense);
	this.movement = new PipBar(player.character.stats.movement);
	this.hp.y = this.name.y + 40;
	this.gun.y = this.hp.y + 25;
	this.melee.y = this.gun.y + 25;

	this.defense.x = this.hp.x + (this.max_stat * 9) + 20; //width of 1st column + spacing
	this.defense.y = this.hp.y;
	this.movement.x = this.gun.x + (this.max_stat * 9) + 20;
	this.movement.y = this.gun.y;
	this.addChild(this.name, this.hp, this.gun, this.melee, this.defense, this.movement);
};

Stats.prototype.update = function(player, stat) {
	stat = stat || '';

	switch(stat){
		case 'hp':
			compUpdate(player.character.stats.hp, this.hp);
			break;
		case 'gun':
			compUpdate(player.character.stats.gun, this.gun);
			break;
		case 'melee':
			compUpdate(player.character.stats.melee, this.melee);
			break;
		case 'defense':
			compUpdate(player.character.stats.defense, this.defense);
			break;
		case 'movement':
			compUpdate(player.character.stats.movement, this.movement);
			break;
		default:
			//Update all
			console.log("updating all pips");
			compUpdate(player.character.stats.hp, this.hp);
			compUpdate(player.character.stats.gun, this.gun);
			compUpdate(player.character.stats.melee, this.melee);
			compUpdate(player.character.stats.defense, this.defense);
			compUpdate(player.character.stats.movement, this.movement);
			break;
	}
}

_extends(Stats, createjs.Container);

var PipBar = function(pipAmt, isPlayer) {
	this._super.constructor.call(this);
	this.isPlayer = isPlayer || true; //change this later
	this.add(pipAmt);
	return this;
};

PipBar.prototype.add = function(num) {
	var x;
	var pips = this.numChildren;
	if(pips == 0) x = 0;
	else {
		var lastPip = this.getChildAt(pips-1);
		x = lastPip.x;
	}
	for(var i = 0; i < num; i++){
		var pip = new createjs.Shape();
		//TODO; specify dimensions more abstractly
		if(this.isPlayer) pip.graphics.beginFill("DimGray").drawRoundRect((11*i)+x, 0, 9, 20, 5);
		else pip.graphics.beginFill("DimGray").drawRoundRect((10*i)+x, 0, 8, 13, 4);
		this.addChild(pip);
	}
};

PipBar.prototype.remove = function(num) {
	var x;
	var pips = this.numChildren;
	if(pips == 0) x = 0;
	else {
		var lastPip = this.getChildAt(pips-1);
		x = lastPip.x;
	}
	for(var i = 0; i < num; i++){
		var pip = this.getChildAt((pips-1)-i);
		//TODO; specify dimensions more abstractly
		if(this.isPlayer) pip.graphics.graphics.clear().beginFill("DimGray").drawRoundRect(
			(11*i)+x, 0, 9, 20, 5);
		else pip.graphics.graphics.clear().beginFill("DimGray").drawRoundRect(
			(10*i)+x, 0, 8, 13, 4);
	}
};

PipBar.prototype.addBuff = function(num) {

};

PipBar.prototype.removeBuff = function(num) {

};

_extends(PipBar, createjs.Container);


function compUpdate(player_stats, pipbar) {
	if(player_stats < pipbar.numChildren){
		pipbar.remove(pipbar.numChildren - player_stats);
	} else {
		pipbar.add(player_stats - pipbar.numChildren);
	}
}