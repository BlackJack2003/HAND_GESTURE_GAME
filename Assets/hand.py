import time,random,socket,subprocess
import mediapipe as mp
import cv2
import numpy as np

import pickle
import sys

#Pi conn desabled for now

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_hands = mp.solutions.hands
'''try:
    s = socket.socket()
    print("Socket created")
except socket.error as err:
    print("Socket failed with error %s"%err)
    quit()'''

port = 8021
addr = "127.0.0.1"
if len(sys.argv)==1:
	print("Connecting to localhost as an argument was given\n Sleeping for 4....")
	time.sleep(5)
else:
	try:
		addr = subprocess.check_output(['ping','raspberrypi.local','-c 2']).decode('utf-8')[24:37]
		print(addr)
	except:
		print('Pi Not found')
		addr = "127.0.0.1"
		quit()

s=  socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.bind((addr,port))
print("Now listening")
s.listen(1)
conn, addr = s.accept()
print(f"Connected by {addr}")

'''s.connect((addr,port))
s.settimeout(2)'''

def sendlr(a):
	msg = str(a)
	print(a)
	msgb = bytes(msg,'utf-8')
	try:
		conn.send(msgb)
		#p = s.recv(1)
		#print(f"Got:{p.decode('utf-8')}")
	except socket.error as err:
		print(f"Network Error:{err}")
		quit()

def open_fing(top,mid,cp):
	top_sums = 0
	for k in range(0,3):
		top_sums+=(top[k]-cp[k])**2
	mid_sums = 0
	for k in range(0,3):
		mid_sums+=(top[k]-cp[k])**2
	if top_sums>=mid_sums:
		return True
	else:
		return False

def get_finger_state(m):
	thumb = [[m.landmark[4].x,m.landmark[4].y,m.landmark[4].z],[m.landmark[3].x,m.landmark[3].y,m.landmark[3].z]]
	iindex = [[m.landmark[8].x,m.landmark[8].y,m.landmark[8].z],[m.landmark[7].x,m.landmark[7].y,m.landmark[7].z]]
	middle_top = [[m.landmark[12].x,m.landmark[12].y,m.landmark[12].z],[m.landmark[11].x,m.landmark[11].y,m.landmark[11].z]]
	ring = [[m.landmark[16].x,m.landmark[16].y,m.landmark[16].z], [m.landmark[15].x,m.landmark[15].y,m.landmark[15].z]]
	little =[ [m.landmark[20].x,m.landmark[20].y,m.landmark[20].z],[m.landmark[19].x,m.landmark[19].y,m.landmark[19].z]]
	center_point = [m.landmark[0].x,m.landmark[0].y,m.landmark[0].z]
	han = [center_point,thumb,iindex,middle_top,ring,little]
	hands = []
	for m in range(5):
		hands.append(True)
	if han[1][0][0]<=han[1][1][0]:
		hands[0] = True
	else:
		hands[0]= False
	for m in range(2,6):
		if han[m][0][1]<=han[m][1][1]:
			hands[m-1] = False
		else:
			hands[m-1] = True
	'''for i in range(3,len(han)):
		hands[i-1] = open_fing(han[i][0],han[i][1],han[0])'''
	return hands

def countfing(results,prev_state,send_count):
	s_c = send_count
	res = results.multi_hand_landmarks
	#handn = results.multi_handedness
	for m in res:
		hands = get_finger_state(m)
		#if hands!= prev_state or send_count>=5:
		hands_seq=[[[True,False,False,True,True],7]]
		if hands[0] == True:
			send_quit = True
			for m in range(1,len(hands)):
				if hands[m] == False:
					send_quit= False
					break
			if send_quit== True:
				print()
				sendlr(6)
				s.close()
				quit()	
			else:
				print("Thumbs away")
				sendlr(1)
		for m in hands_seq:
			if hands == m[0]:
				sendlr(m[1])
				time.sleep(.5)
				break
		_=False
		for m in range(1,5):
			if hands[m] == True:
				sendlr(m+1)
				_=True
				break
		if _==False:
			sendlr(0)
		s_c = 0
	return hands,s_c+1		
	
try:
	cap = cv2.VideoCapture(0)
except:
	cap = cv2.VideoCapture(-1)

with mp_hands.Hands(
		model_complexity=1,
		min_detection_confidence=0.5,
		min_tracking_confidence=0.5,
		max_num_hands=1) as hands:
	prev_state = []
	send_counter = 0
	while cap.isOpened():
		success, image = cap.read()
		if not success:
			print("Ignoring empty camera frame.")
			# If loading a video, use 'break' instead of 'continue'.
			continue
		# To improve performance, optionally mark the image as not writeable to
		# pass by reference.
		image.flags.writeable = False
		image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
		results = hands.process(image)
		# Draw the hand annotations on the image.
		image.flags.writeable = True
		image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
		if results.multi_hand_landmarks:
			prev_state,send_counter = countfing(results,prev_state,send_counter)
			for hand_landmarks in results.multi_hand_landmarks:
				mp_drawing.draw_landmarks(
						image,
						hand_landmarks,
						mp_hands.HAND_CONNECTIONS,
						mp_drawing_styles.get_default_hand_landmarks_style(),
						mp_drawing_styles.get_default_hand_connections_style())
		# Flip the image horizontally for a selfie-view display.
			cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
			if cv2.waitKey(5) & 0xFF == 27:
					break
cap.release()