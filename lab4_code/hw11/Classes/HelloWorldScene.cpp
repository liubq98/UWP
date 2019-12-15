#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h"
#pragma execution_character_set("utf-8")

USING_NS_CC;

int flag = 0;
int blood = 100;

Scene* HelloWorld::createScene()
{
    return HelloWorld::create();
}

// Print useful error message instead of segfaulting when files are not there.
static void problemLoading(const char* filename)
{
    printf("Error while loading: %s\n", filename);
    printf("Depending on how you compiled you might have to add 'Resources/' in front of filenames in HelloWorldScene.cpp\n");
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Scene::init() )
    {
        return false;
    }

    visibleSize = Director::getInstance()->getVisibleSize();
    origin = Director::getInstance()->getVisibleOrigin();

	//创建一张贴图
	auto texture = Director::getInstance()->getTextureCache()->addImage("$lucia_2.png");
	//从贴图中以像素单位切割，创建关键帧
	auto frame0 = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(0, 0, 113, 113)));
	//使用第一帧创建精灵
	player = Sprite::createWithSpriteFrame(frame0);
	player->setPosition(Vec2(origin.x + visibleSize.width / 2,
		origin.y + visibleSize.height / 2));
	addChild(player, 3);

	//hp条
	Sprite* sp0 = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(0, 320, 420, 47)));
	Sprite* sp = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(610, 362, 4, 16)));

	//使用hp条设置progressBar
	pT = ProgressTimer::create(sp);
	pT->setScaleX(90);
	pT->setAnchorPoint(Vec2(0, 0));
	pT->setType(ProgressTimerType::BAR);
	pT->setBarChangeRate(Point(1, 0));
	pT->setMidpoint(Point(0, 1));
	pT->setPercentage(100);
	pT->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width, origin.y + visibleSize.height - 2 * pT->getContentSize().height));
	addChild(pT, 1);
	sp0->setAnchorPoint(Vec2(0, 0));
	sp0->setPosition(Vec2(origin.x + pT->getContentSize().width, origin.y + visibleSize.height - sp0->getContentSize().height));
	addChild(sp0, 0);

	// 静态动画
	idle.reserve(1);
	idle.pushBack(frame0);

	// 攻击动画
	attack.reserve(17);
	for (int i = 0; i < 17; i++) {
		auto frame = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(113 * i, 0, 113, 113)));
		attack.pushBack(frame);
	}

	// 可以仿照攻击动画
	// 死亡动画(帧数：22帧，高：90，宽：79）
	auto texture2 = Director::getInstance()->getTextureCache()->addImage("$lucia_dead.png");
	// Todo
	dead.reserve(22);
	for (int i = 0; i < 22; i++)
	{
		auto frame = SpriteFrame::createWithTexture(texture2, CC_RECT_PIXELS_TO_POINTS(Rect(79 * i, 0, 79, 90)));
		dead.pushBack(frame);
	}

	// 运动动画(帧数：8帧，高：101，宽：68）
	auto texture3 = Director::getInstance()->getTextureCache()->addImage("$lucia_forward.png");
	// Todo
	run.reserve(8);
	for (int i = 0; i < 8; i++)
	{
		auto frame = SpriteFrame::createWithTexture(texture3, CC_RECT_PIXELS_TO_POINTS(Rect(68 * i, 0, 68, 101)));
		run.pushBack(frame);
	}

	auto w = Label::createWithTTF("W", "fonts/arial.ttf", 36);
	auto wItem = MenuItemLabel::create(w, CC_CALLBACK_1(HelloWorld::wEvent, this));
	wItem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 250, origin.y + visibleSize.height / 2 - 100));

	auto s = Label::createWithTTF("S", "fonts/arial.ttf", 36);
	auto sItem = MenuItemLabel::create(s, CC_CALLBACK_1(HelloWorld::sEvent, this));
	sItem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 250, origin.y + visibleSize.height / 2 - 150));

	auto a = Label::createWithTTF("A", "fonts/arial.ttf", 36);
	auto aItem = MenuItemLabel::create(a, CC_CALLBACK_1(HelloWorld::aEvent, this));
	aItem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 300, origin.y + visibleSize.height / 2 - 150));

	auto d = Label::createWithTTF("D", "fonts/arial.ttf", 36);
	auto dItem = MenuItemLabel::create(d, CC_CALLBACK_1(HelloWorld::dEvent, this));
	dItem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 200, origin.y + visibleSize.height / 2 - 150));

	auto direction = Menu::create(wItem, sItem, aItem, dItem, NULL);
	direction->setPosition(Vec2::ZERO);
	this->addChild(direction, 1);

	auto x = Label::createWithTTF("X", "fonts/arial.ttf", 36);
	auto xItem = MenuItemLabel::create(x, CC_CALLBACK_1(HelloWorld::xEvent, this));
	xItem->setPosition(Vec2(origin.x + visibleSize.width / 2 + 300, origin.y + visibleSize.height / 2 - 100));

	auto y = Label::createWithTTF("Y", "fonts/arial.ttf", 36);
	auto yItem = MenuItemLabel::create(y, CC_CALLBACK_1(HelloWorld::yEvent, this));
	yItem->setPosition(Vec2(origin.x + visibleSize.width / 2 + 250, origin.y + visibleSize.height / 2 - 150));

	auto action = Menu::create(xItem, yItem, NULL);
	action->setPosition(Vec2::ZERO);
	this->addChild(action, 1);

	dtime = 180;
	CCString* str_time = CCString::createWithFormat("%d", dtime);
	const char* stime = str_time->getCString();
	time = Label::createWithTTF(stime, "fonts/arial.ttf", 36);
	time->setPosition(Vec2(origin.x + visibleSize.width / 2, origin.y + visibleSize.height / 2 + 200));
	this->addChild(time, 1);

	schedule(schedule_selector(HelloWorld::updateCustom), 1.0f, kRepeatForever, 0);

    return true;
}

void HelloWorld::wEvent(cocos2d::Ref* pSender) {
	if ((player->getPosition()).y + 50 > visibleSize.height)
	{
		return;
	}
	auto animation = Animation::createWithSpriteFrames(run, 0.1f);
	auto animate = Animate::create(animation);
	auto playerMove = MoveTo::create(0.5, Vec2((player->getPosition()).x, (player->getPosition()).y + 50));
	player->runAction(playerMove);
	player->runAction(animate);
	
}

void HelloWorld::sEvent(cocos2d::Ref* pSender) {
	if ((player->getPosition()).y - 50 < 0)
	{
		return;
	}
	auto animation = Animation::createWithSpriteFrames(run, 0.1f);
	auto animate = Animate::create(animation);
	auto playerMove = MoveTo::create(0.5, Vec2((player->getPosition()).x, (player->getPosition()).y - 50));
	player->runAction(playerMove);
	player->runAction(animate);
}

void HelloWorld::aEvent(cocos2d::Ref* pSender) {
	if ((player->getPosition()).x - 50 < 0)
	{
		return;
	}
	auto animation = Animation::createWithSpriteFrames(run, 0.1f);
	auto animate = Animate::create(animation);
	auto playerMove = MoveTo::create(0.5, Vec2((player->getPosition()).x - 50, (player->getPosition()).y));
	player->runAction(playerMove);
	player->runAction(animate);
}

void HelloWorld::dEvent(cocos2d::Ref* pSender) {
	if ((player->getPosition()).x + 50 > visibleSize.width)
	{
		return;
	}
	auto animation = Animation::createWithSpriteFrames(run, 0.1f);
	auto animate = Animate::create(animation);
	auto playerMove = MoveTo::create(0.5, Vec2((player->getPosition()).x + 50, (player->getPosition()).y));
	player->runAction(playerMove);
	player->runAction(animate);
}

void HelloWorld::xEvent(cocos2d::Ref* pSender) {
	if (flag == 1)
	{
		return;
	}
	flag = 1;
	auto animation = Animation::createWithSpriteFrames(dead, 0.1f);
	auto animate = Animate::create(animation);
	auto flagChange = CallFunc::create([this]() {
		flag = 0;
	});
	auto sequence = Sequence::create(animate, flagChange, nullptr);
	player->runAction(sequence);
	if (blood - 20 < 0)
	{
		return;
	}
	schedule(schedule_selector(HelloWorld::updateBloodx), 0, 20, 0);
}

void HelloWorld::yEvent(cocos2d::Ref* pSender) {
	if (flag == 1)
	{
		return;
	}
	flag = 1;
	auto animation = Animation::createWithSpriteFrames(attack, 0.1f);
	auto animate = Animate::create(animation);
	auto flagChange = CallFunc::create([this]() {
		flag = 0;
	});
	auto sequence = Sequence::create(animate, flagChange, nullptr);
	player->runAction(sequence);
	if (blood + 20 > 100)
	{
		return;
	}
	schedule(schedule_selector(HelloWorld::updateBloody), 0, 20, 0);
}

void HelloWorld::updateCustom(float dt) {
	dtime--;
	CCString* str_time = CCString::createWithFormat("%d", dtime);
	const char* stime = str_time->getCString();
	time->setString(stime);
}

void HelloWorld::updateBloodx(float dt) {
	blood--;
	pT->setPercentage(blood);
}

void HelloWorld::updateBloody(float dt) {
	blood++;
	pT->setPercentage(blood);
}